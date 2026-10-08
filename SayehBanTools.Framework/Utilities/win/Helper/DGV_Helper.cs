using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    public static class DGV_Helper
    {
        // یک متد جامع که می‌توانید در تمام فرم‌ها از آن استفاده کنید
        public static void ApplyGridFormatting(DataGridView dgv, DataGridViewCellFormattingEventArgs e, Dictionary<string, int> columnsConfig, string trueValue, string falseValue)
        {
            string colName = dgv.Columns[e.ColumnIndex].Name;

            // بررسی اینکه آیا ستون جاری در دیکشنری ما تعریف شده است یا خیر؟
            if (columnsConfig.TryGetValue(colName, out int formatType))
            {
                // ------------- فرمت نوع 1: تاریخ و زمان -------------
                if (formatType == 1 && e.Value is DateTime dtValue)
                {
                    DateTime utcDate = DateTime.SpecifyKind(dtValue, DateTimeKind.Utc);
                    DateTime iranTime;

                    try
                    {
                        TimeZoneInfo iranTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
                        iranTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, iranTimeZone);
                    }
                    catch
                    {
                        iranTime = utcDate.AddHours(3.5);
                    }

                    PersianCalendar pc = new PersianCalendar();
                    e.Value = $"{pc.GetYear(iranTime):0000}/{pc.GetMonth(iranTime):00}/{pc.GetDayOfMonth(iranTime):00} {iranTime:HH:mm:ss}";
                    e.FormattingApplied = true;
                }

                // ------------- فرمت نوع 3: مقادیر منطقی (True/False) -------------
                else if (formatType == 3 && e.Value is bool boolValue)
                {
                    // اگر ستون IsVisible بود، به جای تیک، متن خوانا نشان دهد
                    e.Value = boolValue ? trueValue : falseValue;
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
