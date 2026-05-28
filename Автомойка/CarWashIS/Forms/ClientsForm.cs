using Microsoft.Data.Sqlite;
using CarWashIS.Data;

namespace CarWashIS.Forms;

internal sealed class ClientsForm : BaseEntityForm
{
    private TextBox _lastName = null!;
    private TextBox _firstName = null!;
    private TextBox _phones = null!;

    protected override string SelectSql =>
        "SELECT Код, Фамилия, Имя, Телефоны FROM Клиенты ORDER BY Код";

    public ClientsForm(EntityFormMode mode = EntityFormMode.Full) : base(mode)
    {
        InitializeForm(mode == EntityFormMode.ReadOnly ? "Клиенты (просмотр)" : "Клиенты");
        var panel = CreateInputPanel();
        var top = 16;
        AddLabel(panel, "Фамилия:", top);
        _lastName = AddTextBox(panel, top);
        top += 40;
        AddLabel(panel, "Имя:", top);
        _firstName = AddTextBox(panel, top);
        top += 40;
        AddLabel(panel, "Телефоны:", top);
        _phones = AddTextBox(panel, top);
        ReloadData();
    }

    protected override void ConfigureGrid()
    {
        Grid.DataBindingComplete += (_, _) =>
        {
            SetHeader("Код", "Код");
            SetHeader("Фамилия", "Фамилия");
            SetHeader("Имя", "Имя");
            SetHeader("Телефоны", "Телефоны");
        };
    }

    private void SetHeader(string name, string header)
    {
        if (Grid.Columns.Contains(name))
            Grid.Columns[name].HeaderText = header;
    }

    protected override void LoadSelectedRow()
    {
        if (Grid.CurrentRow == null) return;
        _lastName.Text = Grid.CurrentRow.Cells["Фамилия"].Value?.ToString() ?? "";
        _firstName.Text = Grid.CurrentRow.Cells["Имя"].Value?.ToString() ?? "";
        _phones.Text = Grid.CurrentRow.Cells["Телефоны"].Value?.ToString() ?? "";
    }

    protected override void ClearInputs()
    {
        _lastName.Clear();
        _firstName.Clear();
        _phones.Clear();
    }

    protected override bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(_lastName.Text) || string.IsNullOrWhiteSpace(_firstName.Text))
        {
            MessageBox.Show("Укажите фамилию и имя.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    protected override void InsertRecord()
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Клиенты (Фамилия, Имя, Телефоны) VALUES (@p1, @p2, @p3)",
            new SqliteParameter("@p1", _lastName.Text.Trim()),
            new SqliteParameter("@p2", _firstName.Text.Trim()),
            new SqliteParameter("@p3", _phones.Text.Trim()));
    }

    protected override void UpdateRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery(
            "UPDATE Клиенты SET Фамилия=@p1, Имя=@p2, Телефоны=@p3 WHERE Код=@p4",
            new SqliteParameter("@p1", _lastName.Text.Trim()),
            new SqliteParameter("@p2", _firstName.Text.Trim()),
            new SqliteParameter("@p3", _phones.Text.Trim()),
            new SqliteParameter("@p4", id));
    }

    protected override void DeleteRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Клиенты WHERE Код=@p1", new SqliteParameter("@p1", id));
    }
}
