using System.Data;
using CarWashIS.Data;
using CarWashIS.UI;

namespace CarWashIS.Forms;

internal enum EntityFormMode
{
    Full,
    ReadOnly,
    AddOnly
}

internal abstract class BaseEntityForm : Form
{
    private readonly EntityFormMode _mode;

    protected DataGridView Grid = null!;
    protected BindingSource Binding = new();

    protected abstract string SelectSql { get; }
    protected abstract void ConfigureGrid();
    protected abstract void LoadSelectedRow();
    protected abstract void ClearInputs();
    protected abstract bool ValidateInput();
    protected abstract void InsertRecord();
    protected abstract void UpdateRecord();
    protected abstract void DeleteRecord();

    protected BaseEntityForm(EntityFormMode mode = EntityFormMode.Full)
    {
        _mode = mode;
    }

    protected void InitializeForm(string title, int width = 960, int height = 580)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(width, height);
        ClientSize = new Size(width, height);
        AppTheme.ApplyForm(this);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = AppTheme.Header
        };
        header.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.HeaderText,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0),
            Font = new Font("Segoe UI", 12F, FontStyle.Bold)
        });

        Grid = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = 270,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        AppTheme.StyleGrid(Grid);
        Grid.SelectionChanged += (_, _) => LoadSelectedRow();

        Controls.Add(Grid);
        Controls.Add(header);
        header.BringToFront();
        ConfigureGrid();
        CreateButtons();
    }

    protected Panel CreateInputPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 12, 16, 12),
            AutoScroll = true,
            BackColor = AppTheme.Background
        };
        Controls.Add(panel);
        panel.BringToFront();
        return panel;
    }

    private void CreateButtons()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            Padding = new Padding(12),
            BackColor = AppTheme.Card
        };

        panel.Controls.Add(CreateButton("Обновить", (_, _) => ReloadData(), primary: false));

        if (_mode is EntityFormMode.Full or EntityFormMode.AddOnly)
            panel.Controls.Add(CreateButton("Добавить", (_, _) => SaveNew(), primary: true));
        if (_mode is EntityFormMode.Full)
        {
            panel.Controls.Add(CreateButton("Изменить", (_, _) => SaveEdit(), primary: true));
            panel.Controls.Add(CreateButton("Удалить", (_, _) => RemoveSelected(), primary: false));
        }

        panel.Controls.Add(CreateButton("Закрыть", (_, _) => Close(), primary: false));

        if (_mode == EntityFormMode.ReadOnly)
            SetInputsReadOnly(true);

        Controls.Add(panel);
    }

    private static Button CreateButton(string text, EventHandler onClick, bool primary)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Margin = new Padding(8, 6, 8, 6),
            Padding = new Padding(14, 8, 14, 8)
        };
        if (primary)
            AppTheme.StylePrimaryButton(button);
        else
            AppTheme.StyleSecondaryButton(button);
        button.Click += onClick;
        return button;
    }

    protected void ReloadData()
    {
        var table = DatabaseHelper.ExecuteQuery(SelectSql);
        Binding.DataSource = table;
        Grid.DataSource = Binding;
        if (Grid.Rows.Count > 0)
            Grid.Rows[0].Selected = true;
        else
            ClearInputs();
    }

    private void SaveNew()
    {
        if (!ValidateInput()) return;
        try
        {
            InsertRecord();
            ReloadData();
            MessageBox.Show("Запись добавлена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void SaveEdit()
    {
        if (Grid.CurrentRow == null)
        {
            MessageBox.Show("Выберите запись в таблице.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInput()) return;
        try
        {
            UpdateRecord();
            ReloadData();
            MessageBox.Show("Запись обновлена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void RemoveSelected()
    {
        if (Grid.CurrentRow == null)
        {
            MessageBox.Show("Выберите запись для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        try
        {
            DeleteRecord();
            ReloadData();
        }
        catch (Exception ex)
        {
            ShowError(ex, "Не удалось удалить. Возможно, запись связана с заказами.");
        }
    }

    protected static void ShowError(Exception ex, string? prefix = null)
    {
        var message = string.IsNullOrWhiteSpace(prefix) ? ex.Message : $"{prefix}\n\n{ex.Message}";
        MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected int? GetSelectedId(string columnName)
    {
        if (Grid.CurrentRow?.Cells[columnName].Value is null or DBNull)
            return null;
        return Convert.ToInt32(Grid.CurrentRow.Cells[columnName].Value);
    }

    protected Label AddLabel(Panel panel, string text, int top)
    {
        var label = new Label
        {
            Text = text,
            Left = 12,
            Top = top,
            Width = 200,
            ForeColor = AppTheme.AccentDark
        };
        panel.Controls.Add(label);
        return label;
    }

    protected TextBox AddTextBox(Panel panel, int top, int width = 360)
    {
        var textBox = new TextBox { Left = 220, Top = top - 3, Width = width };
        panel.Controls.Add(textBox);
        return textBox;
    }

    protected ComboBox AddComboBox(Panel panel, int top, int width = 360)
    {
        var combo = new ComboBox
        {
            Left = 220,
            Top = top - 3,
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        panel.Controls.Add(combo);
        return combo;
    }

    protected DateTimePicker AddDatePicker(Panel panel, int top)
    {
        var picker = new DateTimePicker
        {
            Left = 220,
            Top = top - 3,
            Width = 200,
            Format = DateTimePickerFormat.Short
        };
        panel.Controls.Add(picker);
        return picker;
    }

    protected NumericUpDown AddNumeric(Panel panel, int top, decimal max = 1000000, int decimals = 2)
    {
        var numeric = new NumericUpDown
        {
            Left = 220,
            Top = top - 3,
            Width = 200,
            Maximum = max,
            DecimalPlaces = decimals,
            ThousandsSeparator = true
        };
        panel.Controls.Add(numeric);
        return numeric;
    }

    protected virtual void SetInputsReadOnly(bool readOnly)
    {
        foreach (Control control in Controls)
            SetControlReadOnly(control, readOnly);
    }

    private static void SetControlReadOnly(Control control, bool readOnly)
    {
        switch (control)
        {
            case TextBox textBox:
                textBox.ReadOnly = readOnly;
                break;
            case ComboBox comboBox:
                comboBox.Enabled = !readOnly;
                break;
            case NumericUpDown numeric:
                numeric.ReadOnly = readOnly;
                numeric.Enabled = !readOnly;
                break;
            case DateTimePicker picker:
                picker.Enabled = !readOnly;
                break;
            case MaskedTextBox masked:
                masked.ReadOnly = readOnly;
                break;
        }

        foreach (Control child in control.Controls)
            SetControlReadOnly(child, readOnly);
    }
}
