using CarWashIS.Models;
using Microsoft.Data.Sqlite;

namespace CarWashIS.Data;

internal static class DatabaseInitializer
{
    private const int DataVersion = 2;

    public static void Initialize()
    {
        DatabaseHelper.EnsureDatabaseReady();
        CreateTablesIfNeeded();
        EnsureMetaTable();
        ApplyAccessSeedIfNeeded();
        SeedUsersIfEmpty();
    }

    private static void CreateTablesIfNeeded()
    {
        if (!DatabaseHelper.TableExists("Клиенты"))
        {
            DatabaseHelper.ExecuteNonQuery("""
                CREATE TABLE Клиенты (
                    Код INTEGER PRIMARY KEY AUTOINCREMENT,
                    Фамилия TEXT NOT NULL,
                    Имя TEXT NOT NULL,
                    Телефоны TEXT
                )
                """);
        }

        if (!DatabaseHelper.TableExists("Услуги"))
        {
            DatabaseHelper.ExecuteNonQuery("""
                CREATE TABLE Услуги (
                    Код INTEGER PRIMARY KEY AUTOINCREMENT,
                    НазваниеУслуг TEXT NOT NULL,
                    Цена REAL NOT NULL,
                    ВремяМин INTEGER NOT NULL,
                    Описание TEXT
                )
                """);
        }

        if (!DatabaseHelper.TableExists("Сотрудники"))
        {
            DatabaseHelper.ExecuteNonQuery("""
                CREATE TABLE Сотрудники (
                    Код INTEGER PRIMARY KEY AUTOINCREMENT,
                    Имя TEXT NOT NULL,
                    Должность TEXT NOT NULL,
                    Телефон TEXT,
                    ДатаПриема TEXT NOT NULL
                )
                """);
        }

        if (!DatabaseHelper.TableExists("Заказы"))
        {
            DatabaseHelper.ExecuteNonQuery("""
                CREATE TABLE Заказы (
                    КодЗаказа INTEGER PRIMARY KEY AUTOINCREMENT,
                    КодКлиента INTEGER NOT NULL,
                    КодУслуг INTEGER NOT NULL,
                    КодСотрудника INTEGER NOT NULL,
                    ДатаЗаказа TEXT NOT NULL,
                    Время TEXT NOT NULL,
                    Сумма REAL NOT NULL,
                    FOREIGN KEY (КодКлиента) REFERENCES Клиенты(Код),
                    FOREIGN KEY (КодУслуг) REFERENCES Услуги(Код),
                    FOREIGN KEY (КодСотрудника) REFERENCES Сотрудники(Код)
                )
                """);
        }

        if (!DatabaseHelper.TableExists("Пользователи"))
        {
            DatabaseHelper.ExecuteNonQuery("""
                CREATE TABLE Пользователи (
                    Код INTEGER PRIMARY KEY AUTOINCREMENT,
                    Логин TEXT NOT NULL UNIQUE COLLATE NOCASE,
                    Пароль TEXT NOT NULL,
                    Роль TEXT NOT NULL CHECK (Роль IN ('Admin', 'User')),
                    ДатаРегистрации TEXT NOT NULL
                )
                """);
        }
    }

    private static void EnsureMetaTable()
    {
        DatabaseHelper.ExecuteNonQuery("""
            CREATE TABLE IF NOT EXISTS _DbMeta (
                Ключ TEXT PRIMARY KEY,
                Значение TEXT NOT NULL
            )
            """);
    }

    private static int GetDataVersion()
    {
        var value = DatabaseHelper.ExecuteScalar(
            "SELECT Значение FROM _DbMeta WHERE Ключ = 'DataVersion'");
        return value is null or DBNull ? 0 : Convert.ToInt32(value);
    }

    private static void SetDataVersion(int version)
    {
        DatabaseHelper.ExecuteNonQuery(
            """
            INSERT INTO _DbMeta (Ключ, Значение) VALUES ('DataVersion', @v)
            ON CONFLICT(Ключ) DO UPDATE SET Значение = excluded.Значение
            """,
            new SqliteParameter("@v", version.ToString()));
    }

    private static void ApplyAccessSeedIfNeeded()
    {
        if (GetDataVersion() >= DataVersion)
            return;

        ClearBusinessData();
        SeedAccessData();
        SetDataVersion(DataVersion);
    }

    private static void ClearBusinessData()
    {
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Заказы");
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Клиенты");
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Услуги");
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Сотрудники");
        DatabaseHelper.ExecuteNonQuery("""
            DELETE FROM sqlite_sequence
            WHERE name IN ('Клиенты', 'Услуги', 'Сотрудники', 'Заказы')
            """);
    }

    // Данные как в базе Microsoft Access (автомойка).
    private static void SeedAccessData()
    {
        InsertClient(1, "Иванов", "Иван", "523324234");
        InsertClient(2, "Петрова", "Мария", "312434575");
        InsertClient(3, "Орлов", "Сергей", "523234570");
        InsertClient(4, "Симакина", "Марина", "967868440");
        InsertClient(5, "Олинин", "Слава", "696463674");
        InsertClient(6, "Лямин", "Саша", "549592525");
        InsertClient(7, "Сапегин", "Михаил", "993469396");

        InsertService(1, "Мойка кузова", 550, 30, "Бесконтактная мойка кузова");
        InsertService(2, "Мойка подкапотного пространства", 1500, 40, "Паром и химией");
        InsertService(3, "Нанесение воска", 300, 10, "Защита кузова");
        InsertService(4, "Полировка", 800, 40, "Убирает микроцарапины");
        InsertService(5, "Чистка салона", 3000, 120, "Пылесос и влажная уборка");
        InsertService(6, "Чернение резины", 200, 10, "Блеск для шин");
        InsertService(7, "Обклейка фар и стопарей в бронеплёнку", 1000, 45, "Обклейка защитной плёнкой");

        InsertEmployee(1, "Саша", "Мойщик", "534534535", new DateTime(2026, 3, 21));
        InsertEmployee(2, "Дима", "Мойщик", "424234635", new DateTime(2026, 3, 15));
        InsertEmployee(3, "Сергей", "Старший смены", "534535376", new DateTime(2026, 3, 14));
        InsertEmployee(4, "Михаил", "Полировщик", "964563452", new DateTime(2026, 3, 28));
        InsertEmployee(5, "Андрей", "Мойщик", "934535334", new DateTime(2026, 3, 4));
        InsertEmployee(6, "Елена", "Администратор", "923423512", new DateTime(2026, 3, 2));
        InsertEmployee(7, "Константин", "Универсал", "823525255", new DateTime(2026, 3, 1));

        InsertOrder(1, 1, 1, 1, new DateTime(2026, 4, 16), "10:30", 550);
        InsertOrder(3, 1, 3, 4, new DateTime(2026, 3, 5), "14:15", 300);
        InsertOrder(2, 2, 5, 2, new DateTime(2026, 3, 5), "11:00", 3000);
        InsertOrder(4, 3, 2, 1, new DateTime(2026, 3, 14), "15:30", 1500);
        InsertOrder(5, 4, 7, 7, new DateTime(2026, 3, 27), "09:00", 1000);
        InsertOrder(6, 5, 4, 3, new DateTime(2026, 3, 8), "11:45", 800);
        InsertOrder(7, 6, 6, 5, new DateTime(2026, 3, 13), "16:45", 200);
    }

    private static void SeedUsersIfEmpty()
    {
        if (Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Пользователи") ?? 0) > 0)
            return;

        InsertUser("admin", "admin", UserRole.Admin);
        InsertUser("user", "user", UserRole.User);
    }

    private static void InsertUser(string login, string password, UserRole role)
    {
        DatabaseHelper.ExecuteNonQuery(
            """
            INSERT INTO Пользователи (Логин, Пароль, Роль, ДатаРегистрации)
            VALUES (@p1, @p2, @p3, @p4)
            """,
            new SqliteParameter("@p1", login),
            new SqliteParameter("@p2", PasswordHasher.Hash(password)),
            new SqliteParameter("@p3", role == UserRole.Admin ? "Admin" : "User"),
            new SqliteParameter("@p4", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
    }

    private static void InsertClient(int id, string lastName, string firstName, string phone)
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Клиенты (Код, Фамилия, Имя, Телефоны) VALUES (@id, @p1, @p2, @p3)",
            new SqliteParameter("@id", id),
            new SqliteParameter("@p1", lastName),
            new SqliteParameter("@p2", firstName),
            new SqliteParameter("@p3", phone));
    }

    private static void InsertService(int id, string name, decimal price, int minutes, string description)
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Услуги (Код, НазваниеУслуг, Цена, ВремяМин, Описание) VALUES (@id, @p1, @p2, @p3, @p4)",
            new SqliteParameter("@id", id),
            new SqliteParameter("@p1", name),
            new SqliteParameter("@p2", price),
            new SqliteParameter("@p3", minutes),
            new SqliteParameter("@p4", description));
    }

    private static void InsertEmployee(int id, string name, string position, string phone, DateTime hireDate)
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Сотрудники (Код, Имя, Должность, Телефон, ДатаПриема) VALUES (@id, @p1, @p2, @p3, @p4)",
            new SqliteParameter("@id", id),
            new SqliteParameter("@p1", name),
            new SqliteParameter("@p2", position),
            new SqliteParameter("@p3", phone),
            new SqliteParameter("@p4", hireDate.ToString("yyyy-MM-dd")));
    }

    private static void InsertOrder(int orderId, int clientId, int serviceId, int employeeId, DateTime date, string time, decimal sum)
    {
        DatabaseHelper.ExecuteNonQuery(
            """
            INSERT INTO Заказы (КодЗаказа, КодКлиента, КодУслуг, КодСотрудника, ДатаЗаказа, Время, Сумма)
            VALUES (@id, @p1, @p2, @p3, @p4, @p5, @p6)
            """,
            new SqliteParameter("@id", orderId),
            new SqliteParameter("@p1", clientId),
            new SqliteParameter("@p2", serviceId),
            new SqliteParameter("@p3", employeeId),
            new SqliteParameter("@p4", date.ToString("yyyy-MM-dd")),
            new SqliteParameter("@p5", time),
            new SqliteParameter("@p6", sum));
    }
}
