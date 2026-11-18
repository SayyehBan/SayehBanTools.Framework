using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// راست‌چین کردن داده‌های همه جدول‌های برنامه (متن، عدد، مبلغ، تاریخ).
    /// نکته: در جدول RightToLeft، ویندوز چینش را آینه می‌کند؛ یعنی MiddleRight سمت چپ و MiddleLeft سمت راست رسم می‌شود.
    /// به همین دلیل در هنگام رسم هر خانه (CellFormatting، بعد از رویدادهای خود فرم) چینش درست تنظیم می‌شود
    /// تا تنظیم‌های قبلی ستون‌ها (MiddleRight / MiddleCenter) اثری نداشته باشند.
    /// چک‌باکس، دکمه و تصویر وسط‌چین می‌مانند. برای استثنا: Tag ستون = "NoRightAlign".
    /// از طریق EnterNavigation به همه فرم‌های AddFittedForm وصل می‌شود.
    /// </summary>
    public static class GridRightAlign
    {
        public const string SkipColumnTag = "NoRightAlign";
        private static readonly ConditionalWeakTable<DataGridView, object> Attached = new ConditionalWeakTable<DataGridView, object>();

        public static void Attach(DataGridView grid)
        {
            if (grid == null || Attached.TryGetValue(grid, out _)) return;
            Attached.Add(grid, null);
            grid.CellFormatting += Grid_CellFormatting;
        }

        private static void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.CellStyle == null) return;
            var grid = (DataGridView)sender;
            var col = grid.Columns[e.ColumnIndex];
            if (col is DataGridViewCheckBoxColumn || col is DataGridViewButtonColumn || col is DataGridViewImageColumn) return;
            if (col.Tag is string tag && tag == SkipColumnTag) return;
            e.CellStyle.Alignment = grid.RightToLeft == RightToLeft.Yes
                ? DataGridViewContentAlignment.MiddleLeft     // در جدول راست‌به‌چپ = سمت راست
                : DataGridViewContentAlignment.MiddleRight;
        }
    }
}