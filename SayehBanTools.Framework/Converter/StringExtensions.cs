using Newtonsoft.Json;
using SayehBanTools.Framework.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// افزونه مدیریت رشته ها
    /// </summary>
    public static class StringExtensions
    {
        // دیکشنری قوانین نرمال‌سازی برای هر زبان
        private static readonly Dictionary<string, LanguageNormalizationRules> LanguageRules =
            new Dictionary<string, LanguageNormalizationRules>(StringComparer.OrdinalIgnoreCase)
            {
                // فارسی
                {
                    "fa", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>
                        {
                            { "0", "۰" }, { "1", "۱" }, { "2", "۲" }, { "3", "۳" }, { "4", "۴" },
                            { "5", "۵" }, { "6", "۶" }, { "7", "۷" }, { "8", "۸" }, { "9", "۹" }
                        },
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "۰", "0" }, { "۱", "1" }, { "۲", "2" }, { "۳", "3" }, { "۴", "4" },
                            { "۵", "5" }, { "۶", "6" }, { "۷", "7" }, { "۸", "8" }, { "۹", "9" },
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { "ﮎ", "ک" }, { "ﮏ", "ک" }, { "ﮐ", "ک" }, { "ﮑ", "ک" }, { "ك", "ک" },
                            { "ي", "ی" }, { "ھ", "ه" }, { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "۰",
                        CultureName = "fa-IR"
                    }
                },
                // عربی
                {
                    "ar", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>
                        {
                            { "0", "٠" }, { "1", "١" }, { "2", "٢" }, { "3", "٣" }, { "4", "٤" },
                            { "5", "٥" }, { "6", "٦" }, { "7", "٧" }, { "8", "٨" }, { "9", "٩" }
                        },
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { "ی", "ي" }, { "ک", "ك" }, { "ه", "ھ" }, { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "٠",
                        CultureName = "ar-SA"
                    }
                },
                // هندی
                {
                    "hi", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>
                        {
                            { "0", "०" }, { "1", "१" }, { "2", "२" }, { "3", "३" }, { "4", "४" },
                            { "5", "५" }, { "6", "६" }, { "7", "७" }, { "8", "८" }, { "9", "९" }
                        },
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "०", "0" }, { "१", "1" }, { "२", "2" }, { "३", "3" }, { "۴", "4" },
                            { "५", "5" }, { "۶", "6" }, { "७", "7" }, { "۸", "8" }, { "۹", "9" },
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "०",
                        CultureName = "hi-IN"
                    }
                },
                // بنگالی
                {
                    "bn", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>
                        {
                            { "0", "০" }, { "1", "১" }, { "2", "২" }, { "3", "৩" }, { "4", "৪" },
                            { "5", "৫" }, { "6", "৬" }, { "7", "৭" }, { "8", "৮" }, { "9", "৯" }
                        },
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "০", "0" }, { "১", "1" }, { "২", "2" }, { "৩", "3" }, { "৪", "4" },
                            { "৫", "5" }, { "৬", "6" }, { "৭", "7" }, { "৮", "8" }, { "৯", "9" },
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "০",
                        CultureName = "bn-BD"
                    }
                },
                // تایلندی
                {
                    "th", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>
                        {
                            { "0", "๐" }, { "1", "๑" }, { "2", "๒" }, { "3", "๓" }, { "4", "๔" },
                            { "5", "๕" }, { "6", "๖" }, { "7", "๗" }, { "8", "๘" }, { "9", "๙" }
                        },
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "๐", "0" }, { "๑", "1" }, { "๒", "2" }, { "๓", "3" }, { "๔", "4" },
                            { "๕", "5" }, { "๖", "6" }, { "๗", "7" }, { "๘", "8" }, { "๙", "9" },
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "๐",
                        CultureName = "th-TH"
                    }
                },
                // انگلیسی
                {
                    "en", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>(),
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "۰", "0" }, { "۱", "1" }, { "۲", "2" }, { "۳", "3" }, { "۴", "4" },
                            { "۵", "5" }, { "۶", "6" }, { "۷", "7" }, { "۸", "8" }, { "۹", "9" },
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" },
                            { "०", "0" }, { "१", "1" }, { "२", "2" }, { "३", "3" }, { "४", "4" },
                            { "५", "5" }, { "६", "6" }, { "७", "7" }, { "८", "8" }, { "९", "9" },
                            { "০", "0" }, { "১", "1" }, { "২", "2" }, { "৩", "3" }, { "৪", "4" },
                            { "৫", "5" }, { "৬", "6" }, { "৭", "7" }, { "৮", "8" }, { "৯", "9" },
                            { "๐", "0" }, { "๑", "1" }, { "๒", "2" }, { "๓", "3" }, { "๔", "4" },
                            { "๕", "5" }, { "๖", "6" }, { "๗", "7" }, { "๘", "8" }, { "๙", "9" }
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "0",
                        CultureName = "en-US"
                    }
                },
                // پیش‌فرض برای زبان‌های ناشناخته
                {
                    "default", new LanguageNormalizationRules
                    {
                        ToNativeDigits = new Dictionary<string, string>(),
                        ToEnglishDigits = new Dictionary<string, string>
                        {
                            { "۰", "0" }, { "۱", "1" }, { "۲", "2" }, { "۳", "3" }, { "۴", "4" },
                            { "۵", "5" }, { "۶", "6" }, { "۷", "7" }, { "۸", "8" }, { "۹", "9" }, // فارسی
                            { "٠", "0" }, { "١", "1" }, { "٢", "2" }, { "٣", "3" }, { "٤", "4" },
                            { "٥", "5" }, { "٦", "6" }, { "٧", "7" }, { "٨", "8" }, { "٩", "9" }, // عربی
                            { "०", "0" }, { "१", "1" }, { "२", "2" }, { "३", "3" }, { "४", "4" },
                            { "५", "5" }, { "६", "6" }, { "७", "7" }, { "८", "8" }, { "९", "9" }, // هندی
                            { "০", "0" }, { "১", "1" }, { "২", "2" }, { "৩", "3" }, { "৪", "4" },
                            { "৫", "5" }, { "৬", "6" }, { "৭", "7" }, { "৮", "8" }, { "৯", "9" }, // بنگالی
                            { "๐", "0" }, { "๑", "1" }, { "๒", "2" }, { "๓", "3" }, { "๔", "4" },
                            { "๕", "5" }, { "๖", "6" }, { "๗", "7" }, { "๘", "8" }, { "๙", "9" } // تایلندی
                        },
                        CharReplacements = new Dictionary<string, string>
                        {
                            { " ", " " }, { "‌", " " }
                        },
                        DefaultDigit = "0",
                        CultureName = "en-US"
                    }
                }
            };

        // کلاس کمکی برای قوانین نرمال‌سازی
        private class LanguageNormalizationRules
        {
            public Dictionary<string, string> ToNativeDigits { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, string> ToEnglishDigits { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, string> CharReplacements { get; set; } = new Dictionary<string, string>();
            public string DefaultDigit { get; set; } = "0";
            public string CultureName { get; set; } = "en-US";
        }

        /// <summary>
        /// نرمال‌سازی رشته بر اساس کد زبان
        /// </summary>
        public static string NormalizeByLanguage(this string str, string languageCode, bool toNativeDigits = true)
        {
            if (string.IsNullOrEmpty(str))
                return toNativeDigits ? GetDefaultDigit(languageCode) : "0";

            string normalizedAdaptCode = languageCode?.ToLower() ?? "en";
            if (!LanguageRules.TryGetValue(normalizedAdaptCode, out var rules))
            {
                Console.WriteLine($"Warning: Language code '{languageCode}' not found in LanguageRules. Using default rules.");
                rules = LanguageRules["default"];
            }

            var sb = new StringBuilder(str);
            foreach (var replacement in rules.CharReplacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            var digitRules = toNativeDigits ? rules.ToNativeDigits : rules.ToEnglishDigits;
            foreach (var digit in digitRules)
            {
                sb.Replace(digit.Key, digit.Value);
            }

            // اضافه کردن جایگزینی برای نقطه اعشار فارسی به انگلیسی
            if (!toNativeDigits && normalizedAdaptCode == "fa")
            {
                sb.Replace("٫", "."); // تبدیل نقطه اعشار فارسی به انگلیسی
            }

            return sb.ToString();
        }

        /// <summary>
        /// پاک‌سازی رشته با توجه به زبان
        /// </summary>
        public static string CleanString(this string str, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return str.NormalizeByLanguage(languageCode, false).Trim();
        }
        /// <summary>
        /// تمیز کردن برحسب json یکباره
        /// </summary>
        /// <param name="json"></param>
        /// <param name="defaultLanguage"></param>
        /// <returns></returns>
        public static string CleanJsonByLanguage(this string json, string defaultLanguage = "en")
        {
            if (string.IsNullOrEmpty(json))
                return json;

            try
            {
                var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (translations == null)
                    return json;

                var cleanedTranslations = translations.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.CleanString(kvp.Key).CleanForSql() // استفاده از کد زبان به عنوان پارامتر
                );

                return JsonConvert.SerializeObject(cleanedTranslations);
            }
            catch
            {
                // اگر پردازش JSON با خطا مواجه شد، مقدار اصلی را برگردان
                return json;
            }
        }
        /// <summary>
        /// تبدیل رشته به عدد صحیح
        /// </summary>
        public static int ToInt(this string value, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return int.Parse(value.NormalizeByLanguage(languageCode, false));
        }

        /// <summary>
        /// تبدیل رشته به عدد اعشاری
        /// </summary>
        public static decimal ToDecimal(this string value, string languageCode = "en")
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            // استفاده از CultureInfo.InvariantCulture برای اطمینان از تجزیه درست نقطه اعشار انگلیسی
            return decimal.Parse(value.NormalizeByLanguage(languageCode, false), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// فرمت‌بندی عدد صحیح به‌صورت عددی با جداکننده
        /// </summary>
        public static string ToNumeric(this int value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("N0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد اعشاری به‌صورت عددی با جداکننده
        /// </summary>
        public static string ToNumeric(this decimal value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("N0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد صحیح به‌صورت ارز
        /// </summary>
        public static string ToCurrency(this int value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("C0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// فرمت‌بندی عدد اعشاری به‌صورت ارز
        /// </summary>
        public static string ToCurrency(this decimal value, string languageCode = "en")
        {
            var culture = GetCultureInfo(languageCode);
            return value.ToString("C0", culture).NormalizeByLanguage(languageCode, true);
        }

        /// <summary>
        /// حذف جداکننده‌ها از رشته
        /// </summary>
        public static string RemovePoint(this string str)
        {
            return str?.Replace(",", "").Replace(".", "").Replace("/", "") ?? "";
        }

        /// <summary>
        /// پاک‌سازی پیام خطا
        /// </summary>
        public static string Msg(this string str, string languageCode = "en")
        {
            return str?.Replace("Core .Net SqlClient Data Provider-", "").CleanString(languageCode) ?? "";
        }

        /// <summary>
        /// بررسی وجود مقدار
        /// </summary>
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// تبدیل رشته خالی به null
        /// </summary>
        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        /// <summary>
        /// حذف تگ‌های HTML
        /// </summary>
        public static string HtmlTags(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string pattern = @"<[^>]*(>|$)|‌|»|«";
            return Regex.Replace(str.Replace(" ", " ").Trim(), pattern, string.Empty);
        }

        /// <summary>
        /// جایگزینی خطوط جدید با فاصله
        /// </summary>
        public static string ASCII(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string pattern = @"\r\n";
            return Regex.Replace(str.Trim(), pattern, " ");
        }

        /// <summary>
        /// تبدیل فاصله به خط‌تیره
        /// </summary>
        public static string Slug(this string str)
        {
            return str?.Replace(" ", "-") ?? "";
        }

        /// <summary>
        /// حذف خط‌تیره و تبدیل به فاصله
        /// </summary>
        public static string RemoveSlug(this string str)
        {
            return str?.Replace("-", " ") ?? "";
        }
        /// <summary>
        /// جایگزین کردن Coma
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceComa(this string str)
        {
            return str.Replace("/", ",");
        }
        /// <summary>
        /// حذف عبارت wwwroot/ از مسیر بدون حساسیت به حروف کوچک و بزرگ
        /// </summary>
        public static string RemoveDirectWWWROOT(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            // استفاده از Regex جهت اعمال حالت IgnoreCase در .NET Framework
            return Regex.Replace(str, @"wwwroot/", "", RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// جایگزینی ::1 با 127.0.0.1
        /// </summary>
        public static string AnonymousIP(this string str)
        {
            return str.Replace("::1", "127.0.0.1") ?? "";
        }

        /// <summary>
        /// متدهای قدیمی برای سازگاری
        /// </summary>
        public static string En2Fa(this string str) => str.NormalizeByLanguage("fa", true);
        /// <summary>
        /// تبدیل عدد فارسی به انگلیسی
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Fa2En(this string str) => str.NormalizeByLanguage("fa", false);
        /// <summary>
        /// فیکس کردن به زبان فارسی
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FixPersianChars(this string str) => str.NormalizeByLanguage("fa", false);

        /// <summary>
        /// دریافت CultureInfo بر اساس کد زبان
        /// </summary>
        private static CultureInfo GetCultureInfo(string languageCode)
        {
            try
            {
                // نگاشت کدهای زبان به CultureInfo برای همه‌ی ۱۰۰ زبان
                var cultureMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "am", "am-ET" }, // آمهری
                    { "km", "km-KH" }, // خمر
                    { "dz", "dz-BT" }, // زونگخا
                    { "bo", "bo-CN" }, // تبتی
                    { "ti", "ti-ET" }, // تیگرینیا
                    { "af", "af-ZA" }, // آفریکانس
                    { "az", "az-Latn-AZ" }, // آذربایجانی
                    { "jv", "jv-ID" }, // جاوه‌ای
                    { "bs", "bs-Latn-BA" }, // بوسنیایی
                    { "ca", "ca-ES" }, // کاتالان
                    { "cs", "cs-CZ" }, // چکی
                    { "sn", "sn-ZW" }, // شونا
                    { "cy", "cy-GB" }, // ولزی
                    { "da", "da-DK" }, // دانمارکی
                    { "de", "de-DE" }, // آلمانی
                    { "et", "et-EE" }, // استونیایی
                    { "en", "en-US" }, // انگلیسی
                    { "es", "es-ES" }, // اسپانیایی
                    { "eu", "eu-ES" }, // باسک
                    { "fr", "fr-FR" }, // فرانسوی
                    { "ff", "ff-Latn-SN" }, // فولفولده
                    { "ga", "ga-IE" }, // ایرلندی
                    { "sm", "sm-WS" }, // ساموآیی
                    { "gl", "gl-ES" }, // گالیسیایی
                    { "ha", "ha-Latn-NG" }, // هوسا
                    { "hr", "hr-HR" }, // کرواتی
                    { "ig", "ig-NG" }, // ایگبو
                    { "xh", "xh-ZA" }, // خوسا
                    { "zu", "zu-ZA" }, // زولو
                    { "is", "is-IS" }, // ایسلندی
                    { "it", "it-IT" }, // ایتالیایی
                    { "kl", "kl-GL" }, // گرینلندی
                    { "sw", "sw-KE" }, // سواحیلی
                    { "ku", "ku-Arab-IQ" }, // کردی
                    { "lv", "lv-LV" }, // لتونیایی
                    { "to", "to-TO" }, // تونگایی
                    { "lb", "lb-LU" }, // لوکزامبورگی
                    { "lt", "lt-LT" }, // لیتوانیایی
                    { "hu", "hu-HU" }, // مجاری
                    { "mg", "mg-MG" }, // مالاگاسی
                    { "mt", "mt-MT" }, // مالتی
                    { "fj", "fj-FJ" }, // فیجیایی
                    { "nl", "nl-NL" }, // هلندی
                    { "no", "no-NO" }, // نروژی
                    { "uz", "uz-Latn-UZ" }, // ازبکی
                    { "pl", "pl-PL" }, // لهستانی
                    { "pt", "pt-PT" }, // پرتغالی
                    { "ro", "ro-RO" }, // رومانیایی
                    { "st", "st-LS" }, // سوتو
                    { "tn", "tn-ZA" }, // تسوانا
                    { "sq", "sq-AL" }, // آلبانیایی
                    { "sk", "sk-SK" }, // اسلواکی
                    { "sl", "sl-SI" }, // اسلوونیایی
                    { "so", "so-SO" }, // سومالیایی
                    { "fi", "fi-FI" }, // فنلاندی
                    { "sv", "sv-SE" }, // سوئدی
                    { "vi", "vi-VN" }, // ویتنامی
                    { "ve", "ve-ZA" }, // وندا
                    { "tr", "tr-TR" }, // ترکی
                    { "tk", "tk-TM" }, // ترکمنی
                    { "wo", "wo-SN" }, // ولوف
                    { "ts", "ts-ZA" }, // تسونگا
                    { "yo", "yo-NG" }, // یوروبا
                    { "el", "el-GR" }, // یونانی
                    { "bg", "bg-BG" }, // بلغاری
                    { "kk", "kk-KZ" }, // قزاقی
                    { "ky", "ky-KG" }, // قرقیزی
                    { "mk", "mk-MK" }, // مقدونی
                    { "mn", "mn-MN" }, // مغولی
                    { "ru", "ru-RU" }, // روسی
                    { "sr", "sr-Cyrl-RS" }, // صربی
                    { "tg", "tg-TJ" }, // تاجیکی
                    { "uk", "uk-UA" }, // اوکراینی
                    { "hy", "hy-AM" }, // ارمنی
                    { "he", "he-IL" }, // عبری
                    { "ur", "ur-PK" }, // اردو
                    { "ar", "ar-SA" }, // عربی
                    { "ps", "ps-AF" }, // پشتو
                    { "sd", "sd-Arab-PK" }, // سندی
                    { "fa", "fa-IR" }, // فارسی
                    { "ne", "ne-NP" }, // نپالی
                    { "mr", "mr-IN" }, // مراتی
                    { "hi", "hi-IN" }, // هندی
                    { "bn", "bn-BD" }, // بنگالی
                    { "pa", "pa-IN" }, // پنجابی
                    { "gu", "gu-IN" }, // گجراتی
                    { "or", "or-IN" }, // اوریا
                    { "ta", "ta-IN" }, // تامیلی
                    { "te", "te-IN" }, // تلوگو
                    { "kn", "kn-IN" }, // کانارا
                    { "ml", "ml-IN" }, // مالایالم
                    { "si", "si-LK" }, // سینهالی
                    { "th", "th-TH" }, // تایلندی
                    { "lo", "lo-LA" }, // لائوسی
                    { "my", "my-MM" }, // برمه‌ای
                    { "ka", "ka-GE" }, // گرجی
                    { "zh", "zh-CN" }, // چینی
                    { "ko", "ko-KR" }, // کره‌ای
                    { "ja", "ja-JP" } // ژاپنی
                };

                string normalizedCode = languageCode?.ToLower() ?? "en";
                string cultureName = cultureMapping.ContainsKey(normalizedCode)
                    ? cultureMapping[normalizedCode]
                    : normalizedCode;

                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch
            {
                return CultureInfo.InvariantCulture; // پیش‌فرض
            }
        }

        /// <summary>
        /// دریافت رقم پیش‌فرض برای زبان
        /// </summary>
        private static string GetDefaultDigit(string languageCode)
        {
            if (LanguageRules.TryGetValue(languageCode?.ToLower() ?? "en", out var rules))
                return rules.DefaultDigit;
            return "0";
        }
        /// <summary>
        /// حذف کامل تگ‌های HTML و entityهای رایج
        /// </summary>
        public static string StripHtml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string text = Regex.Replace(input, "<.*?>", string.Empty);  // حذف تگ‌ها
            text = Regex.Replace(text, @"&[a-zA-Z]+;", " ");           // حذف entityها (ساده)
            text = Regex.Replace(text, @"&laquo;", "«");
            text = Regex.Replace(text, @"&raquo;", "»");
            text = Regex.Replace(text, @"&zwnj;", "‌");
            text = Regex.Replace(text, @"&nbsp;", " ");
            text = Regex.Replace(text, @"&amp;", "&");

            return text.Trim();
        }
    }


    /*
        // تبدیل مدل به JSON و تمیز کردن هر مقدار بر اساس کد زبانش
        var translationsJson = JsonConvert.SerializeObject(model.Translations)
            .CleanJsonByLanguage();
     */
}