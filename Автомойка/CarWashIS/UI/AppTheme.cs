namespace CarWashIS.UI;

internal static class AppTheme
{
    public static readonly Color Background = Color.FromArgb(236, 244, 248);
    public static readonly Color Header = Color.FromArgb(0, 105, 128);
    public static readonly Color HeaderText = Color.White;
    public static readonly Color Accent = Color.FromArgb(0, 168, 204);
    public static readonly Color AccentDark = Color.FromArgb(0, 88, 110);
    public static readonly Color Card = Color.White;
    public static readonly Color GridHeader = Color.FromArgb(0, 120, 145);
    public static readonly Color GridSelection = Color.FromArgb(180, 230, 245);
    public static readonly Color TextMuted = Color.FromArgb(70, 95, 110);

    public static readonly Color UserHeader = Color.FromArgb(46, 125, 50);
    public static readonly Color UserAccent = Color.FromArgb(76, 175, 80);
    public static readonly Color UserAccentDark = Color.FromArgb(27, 94, 32);
    public static readonly Color UserBackground = Color.FromArgb(241, 248, 241);

    public static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Card;
        button.ForeColor = AccentDark;
        button.FlatAppearance.BorderColor = Accent;
        button.FlatAppearance.BorderSize = 1;
        button.Cursor = Cursors.Hand;
    }

    public static void StyleMenuButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Card;
        button.ForeColor = AccentDark;
        button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        button.FlatAppearance.BorderColor = Color.FromArgb(190, 220, 230);
        button.FlatAppearance.BorderSize = 1;
        button.Cursor = Cursors.Hand;
        button.Margin = new Padding(10);
    }

    public static void StyleUserMenuButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = Card;
        button.ForeColor = UserAccentDark;
        button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        button.FlatAppearance.BorderColor = Color.FromArgb(200, 230, 200);
        button.FlatAppearance.BorderSize = 1;
        button.Cursor = Cursors.Hand;
        button.Margin = new Padding(10);
    }

    public static void StyleUserPrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = UserAccent;
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Card;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = GridHeader;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grid.ColumnHeadersHeight = 36;
        grid.DefaultCellStyle.SelectionBackColor = GridSelection;
        grid.DefaultCellStyle.SelectionForeColor = AccentDark;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 252, 254);
        grid.RowTemplate.Height = 30;
    }

    public static void ApplyForm(Form form)
    {
        form.BackColor = Background;
        form.Font = new Font("Segoe UI", 10F);
    }
}
