using System;
using System.Globalization;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// کمکی‌های مشترک فرم‌های انبار (انتقال، اصلاح موجودی، موجودی و کاردکس):
    /// تاریخ شمسی ↔ روز تقویمی، تبدیل بازه روز محلی به UTC (بالای بازه انحصاری)، پیام‌ها و متن بسته/عدد
    /// </summary>
    public static class InventoryUi
    {
        public static string ToShamsi(DateTime day)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(day):0000}/{pc.GetMonth(day):00}/{pc.GetDayOfMonth(day):00}";
        }

        public static string ToShamsi(DateTime? day) => day.HasValue ? ToShamsi(day.Value) : "";

        public static DateTime? ParseShamsi(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var parts = text.Trim().Split('/');
            int y, m, d;
            if (parts.Length != 3 || !int.TryParse(parts[0], out y) || !int.TryParse(parts[1], out m) || !int.TryParse(parts[2], out d)) return null;
            try { return new PersianCalendar().ToDateTime(y, m, d, 0, 0, 0, 0); }
            catch { return null; }
        }

        /// <summary>شروع روز محلی (شمسی) به UTC</summary>
        public static DateTime? FromDayUtc(DateTime? localDay) => localDay?.Date.ToUniversalTime();

        /// <summary>پایان انحصاری: شروع روز بعد به UTC</summary>
        public static DateTime? ToDayUtcExclusive(DateTime? localDay) => localDay?.Date.AddDays(1).ToUniversalTime();

        /// <summary>«۳ کارتن + ۴ عدد» برای کالاهای بسته‌ای، وگرنه خود تعداد</summary>
        public static string Packs(double quantity, double itemsPerPack, string unitTitle)
        {
            if (itemsPerPack <= 1 || quantity <= 0) return quantity.ToString("0.###");

            // تبدیل صریح هر دو متغیر به decimal برای محاسبات دقیق ریاضی
            decimal decQty = (decimal)quantity;
            decimal decItemsPerPack = (decimal)itemsPerPack;

            decimal packs = Math.Floor(decQty / decItemsPerPack);
            decimal rest = decQty - (packs * decItemsPerPack);

            string unit = string.IsNullOrWhiteSpace(unitTitle) ? "بسته" : unitTitle;

            return rest == 0 ? $"{packs:0} {unit}" : packs == 0 ? $"{rest:0.###} عدد" : $"{packs:0} {unit} + {rest:0.###} عدد";
        }

        public static void Info(string text, string title) =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);

        public static void Warn(string text, string title) =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);

        public static void Error(string text, string title = "خطا") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);

        public static bool Ask(string text, string title, bool defaultYes = false) =>
            MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                defaultYes ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2, MessageBoxOptions.RtlReading) == DialogResult.Yes;

        /// <summary>مبلغ با مالیات درصدی اعشاری (مثلاً 9.5٪)؛ Calculator کتابخانه فقط درصد صحیح می‌پذیرد</summary>
        public static decimal ApplyTax(decimal amount, double percent) =>
            Math.Round(amount * (1m + (decimal)percent / 100m), 0, MidpointRounding.AwayFromZero);

        /// <summary>مبلغ بعد از تخفیف درصدی اعشاری (مثلاً 2.5٪)</summary>
        public static decimal ApplyDiscount(decimal amount, double percent) =>
            Math.Round(amount * (1m - (decimal)percent / 100m), 0, MidpointRounding.AwayFromZero);

        /// <summary>درصد از متن؛ نقطه اعشار حفظ می‌شود (12.5) و ممیز فارسی هم پذیرفته می‌شود</summary>
        public static double ParsePercent(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            string t = text.Replace("%", "").Replace("٪", "").Replace('٫', '.');
            return (double)ParseDecimal(t);
        }

        /// <summary>عدد از متن (جداکننده هزارگان و ارقام فارسی مجاز)</summary>
        public static decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            string t = text.Replace(",", "").Replace("٬", "").Trim();
            t = t.Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
                 .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9').Replace('/', '.');
            decimal v;
            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out v) ? v : 0;
        }
    }
}