using SayehBanTools.Framework.Utilities.win.component.CommonControls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// ورود تاریخ (انقضا / تولید) به شمسی یا میلادی، مثل فرم فاکتور خرید:
    /// چک‌باکس تیک‌دار = «تاریخ شمسی»؛ بدون تیک = «تاریخ میلادی». با ترک کادر، تاریخ میلادی به شمسی تبدیل و چک‌باکس تیک‌دار می‌شود.
    /// سال ۱۹۰۰ به بالا همیشه میلادی حساب می‌شود (حتی اگر چک‌باکس شمسی باشد) و سال کمتر از ۱۷۰۰ شمسی.
    /// متن کادر بعد از ترک همیشه شمسی است، پس بقیه کد (InventoryUi.ParseShamsi) بدون تغییر کار می‌کند.
    /// </summary>
    public static class DateInput
    {
        public const string ShamsiText = "تاریخ شمسی";
        public const string MiladiText = "تاریخ میلادی";

        /// <summary>اتصال کادر تاریخ و چک‌باکس شمسی/میلادی</summary>
        /// <param name="title">نام تاریخ در پیام خطا (مثلاً «تاریخ انقضا»)</param>
        public static void Bind(SmartTextBox txt, CheckBox chk, string title)
        {
            chk.Text = chk.Checked ? ShamsiText : MiladiText;
            chk.CheckedChanged += (s, e) => chk.Text = chk.Checked ? ShamsiText : MiladiText;
            // Leave کادر قبل از اعتبارسنجی داخلی SmartTextBox اجرا می‌شود؛ پس اعتبارسنجی روی متن شمسی انجام می‌شود
            txt.Leave += (s, e) => Normalize(txt, chk, title, true);
        }

        /// <summary>
        /// تبدیل فوری (برای وقتی Enter در کادر، بدون ترک آن، ردیف را اضافه می‌کند).
        /// خروجی false یعنی تاریخ نامعتبر است (پیام داده شده و فوکوس روی کادر رفته).
        /// </summary>
        public static bool Normalize(SmartTextBox txt, CheckBox chk, string title, bool fromLeave = false)
        {
            string text = ToLatinDigits(txt.Text).Trim();
            if (text.Length == 0) return true;

            int y, m, d;
            if (!TryParts(text, out y, out m, out d))
            {
                // تاریخ ناقص: SmartTextBox هنگام ترک کادر خودش پیام می‌دهد
                if (!fromLeave) Invalid(txt, $"{title} ({txt.Text}) کامل نیست. نمونه شمسی: 1406/03/15 ، میلادی: 2027/06/05");
                return false;
            }

            bool miladi = y >= 1900 || (!chk.Checked && y >= 1700);
            if (!miladi)
            {
                if (!chk.Checked) chk.Checked = true;     // سال شمسی با تیک «میلادی»: همان شمسی حساب می‌شود
                if (IsValidShamsi(y, m, d))
                {
                    txt.Text = $"{y:0000}/{m:00}/{d:00}";
                    return true;
                }
                // تاریخ شمسی نامعتبر: هنگام ترک کادر، SmartTextBox خودش پیام می‌دهد
                if (!fromLeave) Invalid(txt, $"{title} ({txt.Text}) معتبر نیست. نمونه: 1406/03/15");
                return false;
            }

            if (IsValidMiladi(y, m, d))
            {
                txt.Text = InventoryUi.ToShamsi(new DateTime(y, m, d));
                chk.Checked = true;
                return true;
            }

            // میلادی نامعتبر؛ اگر به عنوان شمسی معتبر باشد SmartTextBox پیامی نمی‌دهد، پس اینجا پیام داده می‌شود
            if (!fromLeave || IsValidShamsi(y, m, d))
                Invalid(txt, $"{title} ({txt.Text}) به عنوان تاریخ میلادی معتبر نیست. نمونه: 2027/06/05");
            return false;
        }

        /// <summary>
        /// متن تاریخ ستون جدول (بدون چک‌باکس): سال ۱۷۰۰ به بالا میلادی و تبدیل به شمسی؛ خالی = خالی؛ نامعتبر = null
        /// </summary>
        public static string NormalizeToShamsi(string text)
        {
            string t = ToLatinDigits(text ?? "").Trim();
            if (t.Length == 0) return string.Empty;
            int y, m, d;
            if (!TryParts(t, out y, out m, out d)) return null;
            if (y >= 1700) return IsValidMiladi(y, m, d) ? InventoryUi.ToShamsi(new DateTime(y, m, d)) : null;
            return IsValidShamsi(y, m, d) ? $"{y:0000}/{m:00}/{d:00}" : null;
        }

        private static void Invalid(SmartTextBox txt, string message)
        {
            InventoryUi.Warn(message, "خطای تاریخ");
            txt.Focus();
            txt.SelectAll();
        }

        /// <summary>«1406/3/5»، «1406-03-05» یا ۸ رقم «14060305»</summary>
        private static bool TryParts(string text, out int y, out int m, out int d)
        {
            y = m = d = 0;
            string[] parts = text.Split(new[] { '/', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1 && parts[0].Length == 8 && parts[0].All(char.IsDigit))
                parts = new[] { parts[0].Substring(0, 4), parts[0].Substring(4, 2), parts[0].Substring(6, 2) };
            return parts.Length == 3 && parts[0].Length == 4
                && int.TryParse(parts[0], out y) && int.TryParse(parts[1], out m) && int.TryParse(parts[2], out d);
        }

        private static bool IsValidShamsi(int y, int m, int d)
        {
            if (y < 1300 || y > 1600) return false;
            try { new PersianCalendar().ToDateTime(y, m, d, 0, 0, 0, 0); return true; }
            catch { return false; }
        }

        private static bool IsValidMiladi(int y, int m, int d)
        {
            if (y < 1900 || y > 2200 || m < 1 || m > 12 || d < 1) return false;
            return d <= DateTime.DaysInMonth(y, m);
        }

        private static string ToLatinDigits(string s) =>
            new string(s.Select(c => c >= '۰' && c <= '۹' ? (char)('0' + (c - '۰')) : c >= '٠' && c <= '٩' ? (char)('0' + (c - '٠')) : c).ToArray());
    }
}