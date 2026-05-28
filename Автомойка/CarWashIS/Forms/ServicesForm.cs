using Microsoft.Data.Sqlite;
using CarWashIS.Data;

namespace CarWashIS.Forms;

internal sealed class ServicesForm : BaseEntityForm
{
    private TextBox _name = null!;
    private NumericUpDown _price = null!;
    private NumericUpDown _minutes = null!;
    private TextBox _description = null!;

    protected override string SelectSql =>
        "SELECT Код, НазваниеУслуг, Цена, ВремяМин, Описание FROM Услуги ORDER BY Код";

    public ServicesForm(EntityFormMode mode = EntityFormMode.Full) : base(mode)
    {
        InitializeForm(mode == EntityFormMode.ReadOnly ? "Прайс услуг" : "Услуги");
        var panel = CreateInputPanel();
        var top = 16;
        AddLabel(panel, "Название услуги:", top);
        _name = AddTextBox(panel, top, 400);
        top += 40;
        AddLabel(panel, "Цена (₽):", top);
        _price = AddNumeric(panel, top);
        top += 40;
        AddLabel(panel, "Время (мин):", top);
        _minutes = AddNumeric(panel, top, 9999, 0);
        top += 40;
        AddLabel(panel, "Описание:", top);
        _description = AddTextBox(panel, top, 400);
        _description.Height = 80;
        _description.Multiline = true;
        ReloadData();
    }

    protected override void ConfigureGrid()
    {
        Grid.DataBindingComplete += (_, _) =>
        {
            SetHeader("Код", "Код");
            SetHeader("НазваниеУслуг", "Название");
            SetHeader("Цена", "Цена");
            SetHeader("ВремяМин", "Время (мин)");
            SetHeader("Описание", "Описание");
            if (Grid.Columns.Contains("Цена"))
                Grid.Columns["Цена"].DefaultCellStyle.Format = "N2";
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
        _name.Text = Grid.CurrentRow.Cells["НазваниеУслуг"].Value?.ToString() ?? "";
        _price.Value = Convert.ToDecimal(Grid.CurrentRow.Cells["Цена"].Value);
        _minutes.Value = Convert.ToDecimal(Grid.CurrentRow.Cells["ВремяМин"].Value);
        _description.Text = Grid.CurrentRow.Cells["Описание"].Value?.ToString() ?? "";
    }

    protected override void ClearInputs()
    {
        _name.Clear();
        _price.Value = 0;
        _minutes.Value = 30;
        _description.Clear();
    }

    protected override bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            MessageBox.Show("Укажите название услуги.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    protected override void InsertRecord()
    {
        DatabaseHelper.ExecuteNonQuery(
            "INSERT INTO Услуги (НазваниеУслуг, Цена, ВремяМин, Описание) VALUES (@p1, @p2, @p3, @p4)",
            new SqliteParameter("@p1", _name.Text.Trim()),
            new SqliteParameter("@p2", _price.Value),
            new SqliteParameter("@p3", (int)_minutes.Value),
            new SqliteParameter("@p4", _description.Text.Trim()));
    }

    protected override void UpdateRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery(
            "UPDATE Услуги SET НазваниеУслуг=@p1, Цена=@p2, ВремяМин=@p3, Описание=@p4 WHERE Код=@p5",
            new SqliteParameter("@p1", _name.Text.Trim()),
            new SqliteParameter("@p2", _price.Value),
            new SqliteParameter("@p3", (int)_minutes.Value),
            new SqliteParameter("@p4", _description.Text.Trim()),
            new SqliteParameter("@p5", id));
    }

    protected override void DeleteRecord()
    {
        var id = GetSelectedId("Код") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Услуги WHERE Код=@p1", new SqliteParameter("@p1", id));
    }
}
