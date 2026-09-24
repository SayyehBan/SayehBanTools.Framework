using System.Collections.Generic;
using System.Text;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// کلاس برای تمیز کردن و استانداردسازی متن‌های ارسالی به SQL Server
    /// </summary>
    public static class SafeSqlTextCleaner
    {
        // کاراکترهایی که برای فرار (Escape) یا جایگزینی استفاده می‌شوند
        private static readonly Dictionary<string, string> EscapeReplacements = new Dictionary<string, string>
        {
            { "'", "''" },     // تک کوتیشن → دوتا (برای SQL فرار محسوب می‌شود)
            { "\"", "\"\"" },  // دابل کوتیشن
            { "\0", "" },      // نال کاراکتر → حذف
            { "\b", "" },      // بک‌اسپیس → حذف
            { "\f", "" }       // فرم فید → حذف
        };

        // کاراکترهای اضافی جهت تمیزسازی ظاهر متن
        private static readonly Dictionary<string, string> CleanReplacements = new Dictionary<string, string>
        {
            { "\r\n", "\n" },  // خط جدید ویندوز → یونیکس
            { "\r", "\n" },    // خط جدید مک قدیمی → یونیکس
            { "\t", " " }      // تب → فاصله
        };

        /// <summary>
        /// تمیز کردن متن برای ارسال امن به SQL
        /// </summary>
        /// <param name="input">متن ورودی</param>
        /// <param name="preserveNewLines">آیا خط جدید حفظ بشه؟ (پیش‌فرض: خیر)</param>
        /// <returns>متن امن و استاندارد</returns>
        public static string CleanForSql(this string input, bool preserveNewLines = false)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sb = new StringBuilder(input.Trim());

            // 1. فرار از کاراکترهای خطرناک
            foreach (KeyValuePair<string, string> replacement in EscapeReplacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            // 2. مدیریت خطوط جدید و تب‌ها
            foreach (KeyValuePair<string, string> replacement in CleanReplacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            // 3. تبدیل خط جدید به فاصله در صورت نیاز
            if (!preserveNewLines)
            {
                sb.Replace("\n", " ");
            }

            // 4. حذف فاصله‌های متوالی اضافی
            string result = sb.ToString();
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            return result.Trim();
        }

        /// <summary>
        /// فقط فرار از تک کوتیشن (مناسب برای عبارات ساده)
        /// </summary>
        public static string EscapeSingleQuote(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Replace("'", "''");
        }

        /// <summary>
        /// تمیز کردن متن با حفظ خطوط جدید (Enter ها)
        /// </summary>
        public static string CleanForSqlPreserveLines(this string input)
        {
            return input.CleanForSql(preserveNewLines: true);
        }
    }
}