using System;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// ورود ساعت در کادر متنی (HH:mm). DateTimePicker در فرم راست‌به‌چپ ساعت و دقیقه را جابه‌جا نشان می‌دهد (۱۷:۰۰ ← ۰۰:۱۷)،
    /// پس ساعت با کادر متنی چپ‌به‌راست گرفته می‌شود. ورودی‌های مجاز: 8 ، 08 ، 8:30 ، 0830 ، 830 ، 17.30
    /// </summary>
    public static class TimeInput
    {
        public static bool TryParse(string text, out TimeSpan time)
        {
            time = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(text)) return false;
            string t = Normalize(text);
            int h, m = 0;
            int sep = t.IndexOfAny(new[] { ':', '.', '/', '-' });
            if (sep >= 0)
            {
                if (!int.TryParse(t.Substring(0, sep), out h) || !int.TryParse(t.Substring(sep + 1), out m)) return false;
            }
            else
            {
                int n;
                if (!int.TryParse(t, out n)) return false;
                if (t.Length <= 2) h = n;
                else { h = n / 100; m = n % 100; }
            }
            if (h < 0 || h > 23 || m < 0 || m > 59) return false;
            time = new TimeSpan(h, m, 0);
            return true;
        }

        public static string Format(TimeSpan time) => $"{time.Hours:00}:{time.Minutes:00}";

        /// <summary>ارقام فارسی و فاصله‌ها</summary>
        private static string Normalize(string text)
        {
            var t = text.Trim().Replace(" ", "");
            t = t.Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
                 .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9').Replace('٫', '.');
            return t;
        }

        /// <summary>با خروج از کادر، ساعت به شکل استاندارد (08:30) نوشته می‌شود</summary>
        public static void Bind(TextBox txt)
        {
            txt.Leave += (s, e) =>
            {
                TimeSpan t;
                if (TryParse(txt.Text, out t)) txt.Text = Format(t);
            };
        }
    }
}