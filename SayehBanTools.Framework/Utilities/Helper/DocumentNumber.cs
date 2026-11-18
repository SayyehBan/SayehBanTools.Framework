using System;
using System.Globalization;

namespace SayehBanTools.Framework.Utilities.Helper
{
    /// <summary>
    /// پیشوند شماره اسناد بر اساس تنظیم «شماره‌گذاری اسناد»:
    /// 1 = سال (1405 + 0001)، 2 = سال و ماه (140508 + 0001، پیش‌فرض)، 3 = سال و ماه و روز (14050820 + 0001).
    /// چهار رقم شمارنده را کد فراخواننده (مثلاً SP «DocumentCounters_GenerateNumber») اضافه می‌کند.
    /// تاریخ محلی (شمسی) است، نه UTC.
    /// </summary>
    public static class DocumentNumber
    {
        public const byte FormatYear = 1;
        public const byte FormatYearMonth = 2;
        public const byte FormatYearMonthDay = 3;

        /// <summary>
        /// انتخاب فرمت برای فراخواننده (اگر تنظیم نشده باشد سال و ماه استفاده می‌شود).
        /// مثال: DocumentNumber.FormatSelector = () => StoreStaticData.DocumentNumberFormat;
        /// </summary>
        public static Func<byte> FormatSelector { get; set; } = () => FormatYearMonth;

        public static string Prefix() => PrefixFor(FormatSelector(), DateTime.Now);

        public static string PrefixFor(byte format, DateTime localNow)
        {
            var pc = new PersianCalendar();
            string y = pc.GetYear(localNow).ToString("0000"), m = pc.GetMonth(localNow).ToString("00"), d = pc.GetDayOfMonth(localNow).ToString("00");
            switch (format)
            {
                case FormatYear: return y;
                case FormatYearMonthDay: return y + m + d;
                default: return y + m;
            }
        }

        /// <summary>نمونه شماره سند برای نمایش در تنظیمات</summary>
        public static string Sample(byte format) => PrefixFor(format, DateTime.Now) + "0001";

        public static string Title(byte format)
        {
            switch (format)
            {
                case FormatYear: return "سال + شمارنده";
                case FormatYearMonthDay: return "سال، ماه و روز + شمارنده";
                default: return "سال و ماه + شمارنده";
            }
        }
    }
}