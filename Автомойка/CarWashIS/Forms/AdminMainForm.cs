using CarWashIS.Data;
using CarWashIS.UI;

namespace CarWashIS.Forms;

internal sealed class AdminMainForm : Form
{
    public AdminMainForm()
    {
        Text = "Автомойка — панель администратора";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(780, 500);
        ClientSize = new Size(820, 540);
        AppTheme.ApplyForm(this);

        var login = UserSession.Current?.Login ?? "admin";

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            BackColor = AppTheme.Header
        };
        header.Controls.Add(new Label
        {
            Text = "АВТОМОЙКА «БЛЕСК»",
            Dock = DockStyle.Top,
            Height = 52,
            TextAlign = ContentAlignment.BottomCenter,
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = Color.White
        });
        header.Controls.Add(new Label
        {
            Text = $"Панель администратора  •  {login}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 11F),
            ForeColor = Color.FromArgb(200, 235, 245)
        });

        var buttonsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(40, 28, 40, 20),
            BackColor = AppTheme.Background
        };
        for (var i = 0; i < 3; i++)
            buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
        buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        buttonsPanel.Controls.Add(CreateMenuButton("Клиенты", (_, _) => OpenForm(() => new ClientsForm())), 0, 0);
        buttonsPanel.Controls.Add(CreateMenuButton("Услуги", (_, _) => OpenForm(() => new ServicesForm())), 1, 0);
        buttonsPanel.Controls.Add(CreateMenuButton("Сотрудники", (_, _) => OpenForm(() => new EmployeesForm())), 0, 1);
        buttonsPanel.Controls.Add(CreateMenuButton("Заказы", (_, _) => OpenForm(() => new OrdersForm())), 1, 1);
        buttonsPanel.Controls.Add(CreateMenuButton("Сменить пользователя", (_, _) => Logout()), 0, 2);
        buttonsPanel.SetColumnSpan(buttonsPanel.Controls[4], 2);

        var status = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0),
            ForeColor = AppTheme.TextMuted,
            Text = $"Полный доступ к данным  •  {DatabaseHelper.DatabasePath}"
        };

        Controls.Add(buttonsPanel);
        Controls.Add(status);
        Controls.Add(header);
    }

    private static Button CreateMenuButton(string text, EventHandler onClick)
    {
        var button = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Height = 64
        };
        AppTheme.StyleMenuButton(button);
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
