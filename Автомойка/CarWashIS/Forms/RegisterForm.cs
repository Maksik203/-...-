using CarWashIS.Data;
using CarWashIS.Models;
using CarWashIS.UI;

namespace CarWashIS.Forms;

internal sealed class RegisterForm : Form
{
    private readonly TextBox _login = new();
    private readonly TextBox _password = new();
    private readonly TextBox _confirm = new();
    private readonly ComboBox _role = new();

    public RegisterForm()
    {
        Text = "Регистрация";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(420, 400);
        AppTheme.ApplyForm(this);

        var card = new Panel
        {
            Location = new Point(24, 24),
            Size = new Size(372, 352),
            BackColor = AppTheme.Card
        };

        card.Controls.Add(new Label
        {
            Text = "Новый пользователь",
            Location = new Point(24, 20),
            AutoSize = true,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = AppTheme.AccentDark
        });

        AddField(card, "Логин:", _login, 72, false);
        AddField(card, "Пароль:", _password, 120, true);
        AddField(card, "Повтор пароля:", _confirm, 168, true);

        card.Controls.Add(new Label
        {
            Text = "Роль:",
            Location = new Point(24, 216),
            AutoSize = true,
            ForeColor = AppTheme.AccentDark
        });
        _role.Location = new Point(24, 238);
        _role.Width = 324;
        _role.DropDownStyle = ComboBoxStyle.DropDownList;
        _role.Items.AddRange(["Пользователь", "Администратор"]);
        _role.SelectedIndex = 0;
        card.Controls.Add(_role);

        var registerBtn = new Button
        {
            Text = "Зарегистрироваться",
            Location = new Point(24, 280),
            Size = new Size(200, 40)
        };
        AppTheme.StylePrimaryButton(registerBtn);
        registerBtn.Click += (_, _) => DoRegister();

        var cancelBtn = new Button
        {
            Text = "Отмена",
            Location = new Point(232, 280),
            Size = new Size(116, 40)
        };
        AppTheme.StyleSecondaryButton(cancelBtn);
        cancelBtn.Click += (_, _) => Close();

        card.Controls.AddRange([registerBtn, cancelBtn]);
        Controls.Add(card);
    }

    private static void AddField(Panel parent, string labelText, TextBox textBox, int top, bool isPassword)
    {
        parent.Controls.Add(new Label
        {
            Text = labelText,
            Location = new Point(24, top),
            AutoSize = true,
            ForeColor = AppTheme.AccentDark
        });
        textBox.Location = new Point(24, top + 22);
        textBox.Width = 324;
        if (isPassword)
            textBox.UseSystemPasswordChar = true;
        parent.Controls.Add(textBox);
    }

    private void DoRegister()
    {
        var role = _role.SelectedIndex == 1 ? UserRole.Admin : UserRole.User;
        var result = AuthService.TryRegister(_login.Text, _password.Text, _confirm.Text, role);
        MessageBox.Show(
            result.Message,
            result.Success ? "Регистрация" : "Ошибка",
            MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (result.Success)
            Close();
    }
}
