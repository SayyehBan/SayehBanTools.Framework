using SayehBanTools.Framework.ShowDateTime;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    // کلاس کمکی برای تنظیمات اختصاصی هر ستون
    public class GridFormatConfig
    {
        public int FormatType { get; set; }
        public string TrueText { get; set; }
        public string FalseText { get; set; }

        public GridFormatConfig(int formatType, string trueText = "", string falseText = "")
        {
            FormatType = formatType;
            TrueText = trueText;
            FalseText = falseText;
        }
    }

    public static class DGV_Helper
    {
        // =================================================================
        // متد قدیمی (حفظ شده برای اینکه فرم‌های قبلی خطا ندهند)
        // =================================================================
        public static void ApplyGridFormatting(DataGridView dgv, DataGridViewCellFormattingEventArgs e, Dictionary<string, int> columnsConfig, string trueValue, string falseValue)
        {
            string colName = dgv.Columns[e.ColumnIndex].Name;

            if (columnsConfig.TryGetValue(colName, out int formatType))
            {
                if (formatType == 1 && e.Value is DateTime dtValue)
                {
                    e.Value = ConvertDateTime.ConvertToPersianDateTimeUTC(dtValue);
                    e.FormattingApplied = true;
                }
                else if (formatType == 3 && e.Value is bool boolValue)
                {
                    e.Value = boolValue ? trueValue : falseValue;
                    e.FormattingApplied = true;
                }
            }
        }

        // =================================================================
        // متد جدید و پیشرفته (برای تنظیم متن اختصاصی هر ستون)
        // =================================================================
        public static void ApplyGridFormatting(DataGridView dgv, DataGridViewCellFormattingEventArgs e, Dictionary<string, GridFormatConfig> columnsConfig)
        {
            string colName = dgv.Columns[e.ColumnIndex].Name;

            if (columnsConfig.TryGetValue(colName, out GridFormatConfig config))
            {
                if (config.FormatType == 1 && e.Value is DateTime dtValue)
                {
                    e.Value = ConvertDateTime.ConvertToPersianDateTimeUTC(dtValue);
                    e.FormattingApplied = true;
                }
                else if (config.FormatType == 3 && e.Value is bool boolValue)
                {
                    // استفاده از متن‌های اختصاصی تنظیم شده برای همین ستون
                    e.Value = boolValue ? config.TrueText : config.FalseText;
                    e.FormattingApplied = true;
                }
            }
        }
    }
}