using System.Data;
using Microsoft.Data.Sqlite;
using CarWashIS.Data;

namespace CarWashIS.Forms;

internal sealed class OrdersForm : BaseEntityForm
{
    private ComboBox _client = null!;
    private ComboBox _service = null!;
    private ComboBox _employee = null!;
    private DateTimePicker _orderDate = null!;
    private MaskedTextBox _time = null!;
    private NumericUpDown _sum = null!;

    protected override string SelectSql =>
        """
        SELECT z.КодЗаказа, z.КодКлиента,
               c.Фамилия || ' ' || c.Имя AS Клиент,
               z.КодУслуг, u.НазваниеУслуг AS Услуга,
               z.КодСотрудника, s.Имя AS Сотрудник,
               z.ДатаЗаказа, z.Время, z.Сумма
        FROM Заказы z
        INNER JOIN Клиенты c ON z.КодКлиента = c.Код
        INNER JOIN Услуги u ON z.КодУслуг = u.Код
        INNER JOIN Сотрудники s ON z.КодСотрудника = s.Код
        ORDER BY z.КодЗаказа
        """;

    public OrdersForm(EntityFormMode mode = EntityFormMode.Full) : base(mode)
    {
        var title = mode switch
        {
            EntityFormMode.ReadOnly => "Заказы (просмотр)",
            EntityFormMode.AddOnly => "Новый заказ",
            _ => "Заказы"
        };
        InitializeForm(title, 1000, 620);
        BuildInputs();
        LoadLookups();
        ReloadData();
    }

    private void BuildInputs()
    {
        var panel = CreateInputPanel();
        var top = 16;
        AddLabel(panel, "Клиент:", top);
        _client = AddComboBox(panel, top, 420);
        top += 40;
        AddLabel(panel, "Услуга:", top);
        _service = AddComboBox(panel, top, 420);
        _service.SelectedIndexChanged += (_, _) => UpdateSumFromService();
        top += 40;
        AddLabel(panel, "Сотрудник:", top);
        _employee = AddComboBox(panel, top, 420);
        top += 40;
        AddLabel(panel, "Дата заказа:", top);
        _orderDate = AddDatePicker(panel, top);
        top += 40;
        AddLabel(panel, "Время (ЧЧ:ММ):", top);
        _time = new MaskedTextBox
        {
            Left = 220,
            Top = top - 3,
            Width = 80,
            Mask = "00:00",
            Text = "10:00"
        };
        panel.Controls.Add(_time);
        top += 40;
        AddLabel(panel, "Сумма (₽):", top);
        _sum = AddNumeric(panel, top);
    }

    private void LoadLookups()
    {
        var clients = DatabaseHelper.ExecuteQuery("SELECT Код, Фамилия, Имя FROM Клиенты ORDER BY Фамилия");
        clients.Columns.Add("Display", typeof(string));
        foreach (DataRow row in clients.Rows)
            row["Display"] = $"{row["Фамилия"]} {row["Имя"]}";
        _client.DataSource = clients;
        _client.DisplayMember = "Display";
        _client.ValueMember = "Код";

        var services = DatabaseHelper.ExecuteQuery("SELECT Код, НазваниеУслуг, Цена FROM Услуги ORDER BY Код");
        services.Columns.Add("Display", typeof(string));
        foreach (DataRow row in services.Rows)
            row["Display"] = $"{row["НазваниеУслуг"]} ({Convert.ToDecimal(row["Цена"]):N0} ₽)";
        _service.DataSource = services;
        _service.DisplayMember = "Display";
        _service.ValueMember = "Код";

        var employees = DatabaseHelper.ExecuteQuery("SELECT Код, Имя, Должность FROM Сотрудники ORDER BY Имя");
        employees.Columns.Add("Display", typeof(string));
        foreach (DataRow row in employees.Rows)
            row["Display"] = $"{row["Имя"]} — {row["Должность"]}";
        _employee.DataSource = employees;
        _employee.DisplayMember = "Display";
        _employee.ValueMember = "Код";
    }

    private void UpdateSumFromService()
    {
        if (_service.SelectedValue is not int id)
            return;
        var price = DatabaseHelper.ExecuteScalar(
            "SELECT Цена FROM Услуги WHERE Код=@p1",
            new SqliteParameter("@p1", id));
        if (price is not null and not DBNull)
            _sum.Value = Convert.ToDecimal(price);
    }

    protected override void ConfigureGrid()
    {
        Grid.DataBindingComplete += (_, _) =>
        {
            Hide("КодКлиента");
            Hide("КодУслуг");
            Hide("КодСотрудника");
            SetHeader("КодЗаказа", "№");
            SetHeader("Клиент", "Клиент");
            SetHeader("Услуга", "Услуга");
            SetHeader("Сотрудник", "Сотрудник");
            SetHeader("ДатаЗаказа", "Дата");
            SetHeader("Время", "Время");
            SetHeader("Сумма", "Сумма");
            if (Grid.Columns.Contains("Сумма"))
                Grid.Columns["Сумма"].DefaultCellStyle.Format = "N2";
        };
    }

    private void Hide(string name)
    {
        if (Grid.Columns.Contains(name))
            Grid.Columns[name].Visible = false;
    }

    private void SetHeader(string name, string header)
    {
        if (Grid.Columns.Contains(name))
            Grid.Columns[name].HeaderText = header;
    }

    protected override void LoadSelectedRow()
    {
        if (Grid.CurrentRow == null) return;
        _client.SelectedValue = Grid.CurrentRow.Cells["КодКлиента"].Value;
        _service.SelectedValue = Grid.CurrentRow.Cells["КодУслуг"].Value;
        _employee.SelectedValue = Grid.CurrentRow.Cells["КодСотрудника"].Value;
        _orderDate.Value = DateTime.Parse(Grid.CurrentRow.Cells["ДатаЗаказа"].Value?.ToString() ?? DateTime.Today.ToString());
        _time.Text = Grid.CurrentRow.Cells["Время"].Value?.ToString()?.Replace('.', ':') ?? "10:00";
        _sum.Value = Convert.ToDecimal(Grid.CurrentRow.Cells["Сумма"].Value);
    }

    protected override void ClearInputs()
    {
        if (_client.Items.Count > 0) _client.SelectedIndex = 0;
        if (_service.Items.Count > 0) _service.SelectedIndex = 0;
        if (_employee.Items.Count > 0) _employee.SelectedIndex = 0;
        _orderDate.Value = DateTime.Today;
        _time.Text = "10:00";
        UpdateSumFromService();
    }

    protected override bool ValidateInput()
    {
        if (_client.SelectedValue is null || _service.SelectedValue is null || _employee.SelectedValue is null)
        {
            MessageBox.Show("Выберите клиента, услугу и сотрудника.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (!_time.MaskCompleted)
        {
            MessageBox.Show("Укажите время в формате ЧЧ:ММ.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    protected override void InsertRecord()
    {
        DatabaseHelper.ExecuteNonQuery(
            """
            INSERT INTO Заказы (КодКлиента, КодУслуг, КодСотрудника, ДатаЗаказа, Время, Сумма)
            VALUES (@p1, @p2, @p3, @p4, @p5, @p6)
            """,
            new SqliteParameter("@p1", Convert.ToInt32(_client.SelectedValue)),
            new SqliteParameter("@p2", Convert.ToInt32(_service.SelectedValue)),
            new SqliteParameter("@p3", Convert.ToInt32(_employee.SelectedValue)),
            new SqliteParameter("@p4", _orderDate.Value.ToString("yyyy-MM-dd")),
            new SqliteParameter("@p5", _time.Text),
            new SqliteParameter("@p6", _sum.Value));
    }

    protected override void UpdateRecord()
    {
        var id = GetSelectedId("КодЗаказа") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery(
            """
            UPDATE Заказы SET КодКлиента=@p1, КодУслуг=@p2, КодСотрудника=@p3,
            ДатаЗаказа=@p4, Время=@p5, Сумма=@p6 WHERE КодЗаказа=@p7
            """,
            new SqliteParameter("@p1", Convert.ToInt32(_client.SelectedValue)),
            new SqliteParameter("@p2", Convert.ToInt32(_service.SelectedValue)),
            new SqliteParameter("@p3", Convert.ToInt32(_employee.SelectedValue)),
            new SqliteParameter("@p4", _orderDate.Value.ToString("yyyy-MM-dd")),
            new SqliteParameter("@p5", _time.Text),
            new SqliteParameter("@p6", _sum.Value),
            new SqliteParameter("@p7", id));
    }

    protected override void DeleteRecord()
    {
        var id = GetSelectedId("КодЗаказа") ?? throw new InvalidOperationException();
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Заказы WHERE КодЗаказа=@p1", new SqliteParameter("@p1", id));
    }
}
