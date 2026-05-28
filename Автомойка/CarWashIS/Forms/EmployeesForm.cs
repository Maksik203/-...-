using Microsoft.Data.Sqlite;
using CarWashIS.Data;

namespace CarWashIS.Forms;

internal sealed class EmployeesForm : BaseEntityForm
{
    private TextBox _name = null!;
    private ComboBox _position = null!;
    private TextBox _phone = null!;
    private DateTimePicker _hireDate = null!;

    protected override string SelectSql =>
        "SELECT Код, Имя, Должность, Телефон, ДатаПриема FROM Сотрудники ORDER BY Код";

    public EmployeesForm(EntityFormMode mode = EntityFormMode.Full) : base(mode)
    {
        InitializeForm(mode == EntityFormMode.ReadOnly ? "Сотрудники (просмотр)" : "Сотрудники");
        var panel = CreateInputPanel();
        var top = 16;
        AddLabel(panel, "Имя:", top);
        _name = AddTextBox(panel, top);
        top += 40;
        AddLabel(panel, "Должность:", top);
        _position = AddComboBox(panel, top);
        _position.Items.AddRange(["Мойщик", "Старший смены", "Полировщик", "Администратор", "Универсал"]);
        top += 40;
        AddLabel(panel, "Телефон:", top);
        _phone = AddTextBox(panel, top);
        top += 40;
        AddLabel(panel, "Дата приёма:", top);
        _hireDate = AddDatePicker(panel, top);
        ReloadData();
    }

    protected override void ConfigureGrid()
    {
        Grid.DataBindingComplete += (_, _) =>
        {
            SetHeader("Код", "Код");
            SetHeader("Имя", "Имя");
            SetHeader("Должность", "Должность");
            SetHeader("Телефон", "Телефон");
            SetHeader("ДатаПриема", "Дата приёма");
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
        _name.Text = Grid.CurrentRow.Cells["Имя"].Value?.ToString() ?? "";
        var pos = Grid.CurrentRow.Cells["Должность"].Value?.ToString() ?? "";
        _position.SelectedItem = pos;
        if (_position.SelectedIndex < 0) _position.Text = pos;
        _phone.Text = Grid.CurrentRow.Cells["Телефон"].Value?.ToString() ?? "";
        _hireDate.Value = DateTime.Parse(Grid.CurrentRow.Cells["ДатаПриема"].Value?.ToString() ?? DateTime.Today.ToString());
    }

    protected override void ClearInputs()
    {
        _name.Clear();
        _position.SelectedIndex = 0;
        _phone.Clear();
        _hireDate.Value = DateTime.Today;
    }

    protected override bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(_name.Text) || _position.SelectedItem is null)
        {
            MessageBox.Show("Укажите имя и должность.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    protected override void InsertRecord()
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Сотрудники (Имя, Должность, Телефон, ДатаПриема) VALUES (@p1, @p2, @p3, @p4)",
            new SqliteParameter("@p1", _name.Text.Trim()),
            new SqliteParameter("@p2", _position.SelectedItem!.ToString()),
            new SqliteParameter("@p3", _phone.Text.Trim()),
            new SqliteParameter("@p4", _hireDate.Value.ToString("yyyy-MM-dd")));
    }

    protected override void UpdateRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery(
            "UPDATE Сотрудники SET Имя=@p1, Должность=@p2, Телефон=@p3, ДатаПриема=@p4 WHERE Код=@p5",
            new SqliteParameter("@p1", _name.Text.Trim()),
            new SqliteParameter("@p2", _position.SelectedItem!.ToString()),
            new SqliteParameter("@p3", _phone.Text.Trim()),
            new SqliteParameter("@p4", _hireDate.Value.ToString("yyyy-MM-dd")),
            new SqliteParameter("@p5", id));
    }

    protected override void DeleteRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Сотрудники WHERE Код=@p1", new SqliteParameter("@p1", id));
    }
}
