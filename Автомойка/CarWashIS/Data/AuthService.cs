using CarWashIS.Models;
using Microsoft.Data.Sqlite;

namespace CarWashIS.Data;

internal static class AuthService
{
    public static (bool Success, string Message, UserAccount? Account) TryLogin(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return (false, "Введите логин и пароль.", null);

        var table = DatabaseHelper.ExecuteQuery(
            "SELECT Код, Логин, Пароль, Роль FROM Пользователи WHERE Логин = @login COLLATE NOCASE",
            new SqliteParameter("@login", login.Trim()));

        if (table.Rows.Count == 0)
            return (false, "Неверный логин или пароль.", null);

        var row = table.Rows[0];
        var storedHash = row["Пароль"]?.ToString() ?? "";
        if (!PasswordHasher.Verify(password, storedHash))
            return (false, "Неверный логин или пароль.", null);

        var account = new UserAccount
        {
            Id = Convert.ToInt32(row["Код"]),
            Login = row["Логин"]?.ToString() ?? login.Trim(),
            Role = ParseRole(row["Роль"]?.ToString())
        };
        return (true, "Вход выполнен.", account);
    }

    public static (bool Success, string Message) TryRegister(string login, string password, string confirmPassword, UserRole role)
    {
        login = login.Trim();
        if (login.Length < 3)
            return (false, "Логин должен быть не короче 3 символов.");
        if (password.Length < 4)
            return (false, "Пароль должен быть не короче 4 символов.");
        if (password != confirmPassword)
            return (false, "Пароли не совпадают.");

        var exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Пользователи WHERE Логин = @login COLLATE NOCASE",
            new SqliteParameter("@login", login)) ?? 0);
        if (exists > 0)
            return (false, "Пользователь с таким логином уже существует.");

        DatabaseHelper.ExecuteNonQuery(
            """
            INSERT INTO Пользователи (Логин, Пароль, Роль, ДатаРегистрации)
            VALUES (@login, @password, @role, @date)
            """,
            new SqliteParameter("@login", login),
            new SqliteParameter("@password", PasswordHasher.Hash(password)),
            new SqliteParameter("@role", role == UserRole.Admin ? "Admin" : "User"),
            new SqliteParameter("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

        return (true, role == UserRole.Admin
            ? "Администратор зарегистрирован. Теперь можно войти."
            : "Регистрация завершена. Теперь можно войти.");
    }

    private static UserRole ParseRole(string? role) =>
        string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ? UserRole.Admin : UserRole.User;
}
