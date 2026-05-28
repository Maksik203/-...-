using CarWashIS.Data;
using CarWashIS.UI;

namespace CarWashIS.Forms;

internal sealed class LoginForm : Form
{
    private readonly TextBox _login = new();
    private readonly TextBox _password = new();

    public LoginForm()
    {
        Text = "Автомойка — вход";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 380);
        AppTheme.ApplyForm(this);

        var card = new Panel
        {
            Location = new Point(24, 24),
            Size = new Size(372, 332),
            BackColor = AppTheme.Card
        };

        var title = new Label
        {
            Text = "Вход в систему",
            Location = new Point(24, 20),
            AutoSize = true,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = AppTheme.AccentDark
        };

        var subtitle = new Label
        {
            Text = "Автомойка «Блеск»",
            Location = new Point(24, 52),
            AutoSize = true,
            ForeColor = AppTheme.TextMuted
        };

        AddField(card, "Логин:", _login, 88, false);
        AddField(card, "Пароль:", _password, 136, true);

        var hint = new Label
        {
            Text = "По умолчанию: admin / admin или user / user",
            Location = new Point(24, 182),
            Size = new Size(324, 36),
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.5F)
        };

        var loginBtn = new Button
        {
            Text = "Войти",
            Location = new Point(24, 224),
            Size = new Size(160, 40)
        };
        AppTheme.StylePrimaryButton(loginBtn);
        loginBtn.Click += (_, _) => DoLogin();

        var registerBtn = new Button
        {
            Text = "Регистрация",
            Location = new Point(196, 224),
            Size = new Size(152, 40)
        };
        AppTheme.StyleSecondaryButton(registerBtn);
        registerBtn.Click += (_, _) => OpenRegister();

        var exitBtn = new Button
        {
            Text = "Выход",
            Location = new Point(24, 276),
            Size = new Size(324, 40)
        };
        AppTheme.StyleSecondaryButton(exitBtn);
        exitBtn.Click += (_, _) => Close();

        AcceptButton = loginBtn;
        _password.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoLogin();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };

        card.Controls.AddRange([title, subtitle, hint, loginBtn, registerBtn, exitBtn]);
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

    private void DoLogin()
    {
        var result = AuthService.TryLogin(_login.Text, _password.Text);
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Вход", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UserSession.SignIn(result.Account!);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OpenRegister()
    {
        using var form = new RegisterForm();
        form.ShowDialog(this);
    }
}
