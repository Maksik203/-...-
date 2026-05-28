using CarWashIS.Data;
using CarWashIS.Forms;

namespace CarWashIS;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            DatabaseInitializer.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не удалось открыть базу данных.\n\n{ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        while (true)
        {
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK)
                break;

            using var main = UserSession.IsAdmin
                ? (Form)new AdminMainForm()
                : new UserMainForm();

            main.ShowDialog();
            UserSession.SignOut();
        }
    }
}
