using CarWashIS.Data;
using CarWashIS.UI;

namespace CarWashIS.Forms;

internal sealed class UserMainForm : Form
{
    public UserMainForm()
    {
        Text = "Автомойка — личный кабинет";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 480);
        ClientSize = new Size(760, 500);
        BackColor = AppTheme.UserBackground;
        Font = new Font("Segoe UI", 10F);

        var login = UserSession.Current?.Login ?? "user";

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 130,
            BackColor = AppTheme.UserHeader
        };
        header.Controls.Add(new Label
        {
            Text = "ЛИЧНЫЙ КАБИНЕТ",
            Dock = DockStyle.Top,
            Height = 56,
            TextAlign = ContentAlignment.BottomCenter,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.White
        });
        header.Controls.Add(new Label
        {
            Text = $"Добро пожаловать, {login}!",
            Dock = DockStyle.Top,
            Height = 32,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F),
            ForeColor = Color.FromArgb(220, 245, 220)
        });
        header.Controls.Add(new Label
        {
            Text = "Просмотр услуг и оформление заказов",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopCenter,
            ForeColor = Color.FromArgb(200, 235, 200)
        });

        var info = new Panel
        {
            Dock = DockStyle.Top,
            Height = 72,
            Padding = new Padding(24, 12, 24, 8),
            BackColor = AppTheme.UserBackground
        };
        info.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Вы вошли как обычный пользователь. Доступны просмотр прайса, списка заказов и оформление нового заказа. Редактирование справочников недоступно.",
            ForeColor = AppTheme.UserAccentDark,
            Font = new Font("Segoe UI", 9.5F)
        });

        var buttonsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(36, 16, 36, 20),
            BackColor = AppTheme.UserBackground
        };
        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        buttonsPanel.Controls.Add(CreateUserButton("Прайс услуг", (_, _) => OpenForm(() => new ServicesForm(EntityFormMode.ReadOnly))), 0, 0);
        buttonsPanel.Controls.Add(CreateUserButton("Список заказов", (_, _) => OpenForm(() => new OrdersForm(EntityFormMode.ReadOnly))), 1, 0);
        buttonsPanel.Controls.Add(CreateUserButton("Оформить заказ", (_, _) => OpenForm(() => new OrdersForm(EntityFormMode.AddOnly))), 0, 1);
        buttonsPanel.Controls.Add(CreateUserButton("Сменить пользователя", (_, _) => Logout()), 1, 1);

        Controls.Add(buttonsPanel);
        Controls.Add(info);
        Controls.Add(header);
    }

    private static Button CreateUserButton(string text, EventHandler onClick)
    {
        var button = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Height = 72
        };
        AppTheme.StyleUserMenuButton(button);
        button.Click += onClick;
        return button;
    }

    private static void OpenForm(Func<Form> factory)
    {
        try
        {
            using var form = factory();
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Logout()
    {
        UserSession.SignOut();
        Close();
    }
}
