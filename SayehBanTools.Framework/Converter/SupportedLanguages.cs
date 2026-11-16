using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// اطلاعات یک زبان پشتیبانی‌شده (کد، منطقه، نام بومی، جهت متن و رقم بومی)
    /// </summary>
    public sealed class LanguageInfo
    {
        internal LanguageInfo(int id, string code, string regionCode, string nativeName, bool isRtl,
            char? nativeDigitZero, string[] cultureCandidates, Dictionary<char, char> charReplacements,
            bool preserveJoiners)
        {
            Id = id;
            Code = code;
            RegionCode = regionCode;
            NativeName = nativeName;
            IsRtl = isRtl;
            NativeDigitZero = nativeDigitZero;
            CultureCandidates = cultureCandidates;
            CharReplacements = charReplacements;
            PreserveJoiners = preserveJoiners;
        }

        /// <summary>شناسه زبان (مطابق جدول Languages)</summary>
        public int Id { get; }

        /// <summary>کد دوحرفی زبان (ISO 639-1) مثل fa</summary>
        public string Code { get; }

        /// <summary>کد زبان به همراه منطقه مثل fa-IR</summary>
        public string RegionCode { get; }

        /// <summary>نام زبان به خط و زبان خودش</summary>
        public string NativeName { get; }

        /// <summary>آیا زبان راست‌به‌چپ است</summary>
        public bool IsRtl { get; }

        /// <summary>جهت متن: rtl یا ltr</summary>
        public string TextDirection => IsRtl ? "rtl" : "ltr";

        /// <summary>کاراکتر صفرِ ارقام بومی زبان؛ اگر زبان رقم بومی نداشته باشد null است (ارقام لاتین)</summary>
        public char? NativeDigitZero { get; }

        /// <summary>آیا زبان ارقام اختصاصی خودش را دارد</summary>
        public bool HasNativeDigits => NativeDigitZero.HasValue;

        /// <summary>رقم صفر پیش‌فرض زبان</summary>
        public string DefaultDigit => NativeDigitZero.HasValue ? NativeDigitZero.Value.ToString() : "0";

        internal string[] CultureCandidates { get; }
        internal Dictionary<char, char> CharReplacements { get; }

        /// <summary>
        /// در خط‌هایی مثل هندی، بنگالی، تایلندی و ... کاراکترهای ZWNJ/ZWJ در شکل‌دهی حروف نقش دارند و نباید حذف شوند
        /// </summary>
        internal bool PreserveJoiners { get; }

        private CultureInfo _culture;

        /// <summary>
        /// CultureInfo متناظر با زبان (در صورت نبود در سیستم‌عامل، InvariantCulture)
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                if (_culture != null)
                    return _culture;

                CultureInfo resolved = CultureInfo.InvariantCulture;
                foreach (var name in CultureCandidates)
                {
                    try
                    {
                        resolved = CultureInfo.GetCultureInfo(name);
                        break;
                    }
                    catch (CultureNotFoundException)
                    {
                        // نام بعدی را امتحان کن
                    }
                }

                _culture = resolved;
                return resolved;
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"{Code} ({RegionCode}) {NativeName}";
    }

    /// <summary>
    /// فهرست ۹۹ زبان پشتیبانی‌شده در کتابخانه
    /// </summary>
    public static class SupportedLanguages
    {
        // صفرِ ارقام بومی هر خط
        private const char ArabicIndic = '٠';         // ٠ عربی
        private const char ExtendedArabicIndic = '۰'; // ۰ فارسی/اردو/پشتو
        private const char Devanagari = '०';          // ० هندی/مراتی/نپالی
        private const char Bengali = '০';             // ০
        private const char Gurmukhi = '੦';            // ੦ پنجابی
        private const char Gujarati = '૦';            // ૦
        private const char Oriya = '୦';               // ୦
        private const char Tamil = '௦';               // ௦
        private const char Telugu = '౦';              // ౦
        private const char Kannada = '೦';             // ೦
        private const char Malayalam = '൦';           // ൦
        private const char Thai = '๐';                // ๐
        private const char Lao = '໐';                 // ໐
        private const char Tibetan = '༠';             // ༠
        private const char Myanmar = '၀';             // ၀
        private const char Khmer = '០';               // ០

        // قوانین جایگزینی حروف خط عربی
        private static readonly Dictionary<char, char> PersianChars = new Dictionary<char, char>
        {
            { 'ﮎ', 'ک' }, { 'ﮏ', 'ک' }, { 'ﮐ', 'ک' }, { 'ﮑ', 'ک' }, { 'ك', 'ک' },
            { 'ي', 'ی' }, { 'ى', 'ی' }, { 'ھ', 'ه' }
        };

        private static readonly Dictionary<char, char> ArabicChars = new Dictionary<char, char>
        {
            { 'ی', 'ي' }, { 'ک', 'ك' }
        };

        private static readonly Dictionary<char, char> UrduChars = new Dictionary<char, char>
        {
            { 'ك', 'ک' }, { 'ي', 'ی' }, { 'ى', 'ی' }
        };

        // پشتو حروف ی، ي، ې و ۍ را جدا دارد، پس فقط کاف یکسان‌سازی می‌شود
        private static readonly Dictionary<char, char> PashtoChars = new Dictionary<char, char>
        {
            { 'ك', 'ک' }
        };

        // سندی برای «ک» از ڪ استفاده می‌کند (ک برای «کھ» است) و «ي» عربی را به کار می‌برد
        private static readonly Dictionary<char, char> SindhiChars = new Dictionary<char, char>
        {
            { 'ك', 'ڪ' }, { 'ی', 'ي' }
        };

        private static readonly Dictionary<char, char> NoChars = new Dictionary<char, char>();

        private static readonly LanguageInfo[] All =
        {
            L(1,  "fa", "fa-IR", "پارسی",            true,  ExtendedArabicIndic, PersianChars),
            L(2,  "en", "en-US", "English",          false),
            L(3,  "ar", "ar-SA", "العربية",          true,  ArabicIndic, ArabicChars),
            L(4,  "zh", "zh-CN", "中文",             false),
            L(5,  "es", "es-ES", "Español",          false),
            L(6,  "hi", "hi-IN", "हिन्दी",            false, Devanagari),
            L(7,  "bn", "bn-BD", "বাংলা",             false, Bengali),
            L(8,  "pt", "pt-PT", "Português",        false),
            L(9,  "ru", "ru-RU", "Русский",          false),
            L(10, "ja", "ja-JP", "日本語",           false),
            L(11, "pa", "pa-IN", "ਪੰਜਾਬੀ",           false, Gurmukhi,  null, "pa-Guru-IN"),
            L(12, "de", "de-DE", "Deutsch",          false),
            L(13, "jv", "jv-ID", "Basa Jawa",        false, null,      null, "jv-Latn-ID", "jv-Latn"),
            L(14, "ko", "ko-KR", "한국어",           false),
            L(15, "fr", "fr-FR", "Français",         false),
            L(16, "te", "te-IN", "తెలుగు",            false, Telugu),
            L(17, "vi", "vi-VN", "Tiếng Việt",       false),
            L(18, "mr", "mr-IN", "मराठी",            false, Devanagari),
            L(19, "ta", "ta-IN", "தமிழ்",            false, Tamil),
            L(20, "ur", "ur-PK", "اردو",             true,  ExtendedArabicIndic, UrduChars),
            L(21, "tr", "tr-TR", "Türkçe",           false),
            L(22, "it", "it-IT", "Italiano",         false),
            L(23, "th", "th-TH", "ไทย",              false, Thai),
            L(24, "gu", "gu-IN", "ગુજરાતી",           false, Gujarati),
            L(25, "uk", "uk-UA", "Українська",       false),
            L(26, "pl", "pl-PL", "Polski",           false),
            L(27, "ml", "ml-IN", "മലയാളം",           false, Malayalam),
            L(28, "kn", "kn-IN", "ಕನ್ನಡ",             false, Kannada),
            L(29, "or", "or-IN", "ଓଡ଼ିଆ",             false, Oriya),
            L(30, "my", "my-MM", "မြန်မာဘာသာ",       false, Myanmar),
            L(31, "sw", "sw-TZ", "Kiswahili",        false, null,      null, "sw-KE"),
            L(32, "am", "am-ET", "አማርኛ",             false),
            L(33, "yo", "yo-NG", "Yorùbá",           false),
            L(34, "uz", "uz-UZ", "Oʻzbek",           false, null,      null, "uz-Latn-UZ"),
            L(35, "az", "az-AZ", "Azərbaycan",       false, null,      null, "az-Latn-AZ"),
            L(36, "ha", "ha-NG", "Hausa",            false, null,      null, "ha-Latn-NG"),
            L(37, "ig", "ig-NG", "Igbo",             false),
            L(38, "nl", "nl-NL", "Nederlands",       false),
            L(39, "sv", "sv-SE", "Svenska",          false),
            L(40, "cs", "cs-CZ", "Čeština",          false),
            L(41, "ro", "ro-RO", "Română",           false),
            L(42, "hu", "hu-HU", "Magyar",           false),
            L(43, "el", "el-GR", "Ελληνικά",         false),
            L(44, "he", "he-IL", "עברית",            true),
            L(45, "da", "da-DK", "Dansk",            false),
            L(46, "fi", "fi-FI", "Suomi",            false),
            L(47, "sk", "sk-SK", "Slovenčina",       false),
            L(48, "no", "no-NO", "Norsk",            false, null,      null, "nb-NO"),
            L(49, "bg", "bg-BG", "Български",        false),
            L(50, "sr", "sr-RS", "Српски",           false, null,      null, "sr-Cyrl-RS"),
            L(51, "hr", "hr-HR", "Hrvatski",         false),
            L(52, "lt", "lt-LT", "Lietuvių",         false),
            L(53, "lv", "lv-LV", "Latviešu",         false),
            L(54, "et", "et-EE", "Eesti",            false),
            L(55, "sl", "sl-SI", "Slovenščina",      false),
            L(56, "mk", "mk-MK", "Македонски",       false),
            L(57, "bs", "bs-BA", "Bosanski",         false, null,      null, "bs-Latn-BA"),
            L(58, "sq", "sq-AL", "Shqip",            false),
            L(59, "ka", "ka-GE", "ქართული",          false),
            L(60, "hy", "hy-AM", "Հայերեն",          false),
            L(61, "mn", "mn-MN", "Монгол",           false),
            L(62, "km", "km-KH", "ខ្មែរ",              false, Khmer),
            L(63, "lo", "lo-LA", "ລາວ",              false, Lao),
            L(64, "si", "si-LK", "සිංහල",            false, null,      null),
            L(65, "ne", "ne-NP", "नेपाली",            false, Devanagari),
            L(66, "ps", "ps-AF", "پښتو",             true,  ExtendedArabicIndic, PashtoChars),
            L(67, "sd", "sd-PK", "سنڌي",             true,  ArabicIndic, SindhiChars, "sd-Arab-PK"),
            L(68, "ku", "ku-IQ", "Kurdî",            false, null,      null, "ku-Arab-IQ"),
            L(69, "tg", "tg-TJ", "Тоҷикӣ",           false, null,      null, "tg-Cyrl-TJ"),
            L(70, "ky", "ky-KG", "Кыргызча",         false),
            L(71, "tk", "tk-TM", "Türkmençe",        false),
            L(72, "kk", "kk-KZ", "Қазақша",          false),
            L(73, "mt", "mt-MT", "Malti",            false),
            L(74, "is", "is-IS", "Íslenska",         false),
            L(75, "ga", "ga-IE", "Gaeilge",          false),
            L(76, "cy", "cy-GB", "Cymraeg",          false),
            L(77, "eu", "eu-ES", "Euskara",          false),
            L(78, "gl", "gl-ES", "Galego",           false),
            L(79, "ca", "ca-ES", "Català",           false),
            L(80, "lb", "lb-LU", "Lëtzebuergesch",   false),
            L(81, "sm", "sm-WS", "Gagana Samoa",     false),
            L(82, "to", "to-TO", "Lea Faka-Tonga",   false),
            L(83, "fj", "fj-FJ", "Na Vosa Vakaviti", false),
            L(84, "mg", "mg-MG", "Malagasy",         false),
            L(85, "af", "af-ZA", "Afrikaans",        false),
            L(86, "zu", "zu-ZA", "isiZulu",          false),
            L(87, "xh", "xh-ZA", "isiXhosa",         false),
            L(88, "st", "st-LS", "Sesotho",          false, null,      null, "st-ZA"),
            L(89, "tn", "tn-BW", "Setswana",         false, null,      null, "tn-ZA"),
            L(90, "ts", "ts-ZA", "Xitsonga",         false),
            L(91, "ve", "ve-ZA", "Tshivenḓa",        false),
            L(92, "sn", "sn-ZW", "chiShona",         false, null,      null, "sn-Latn-ZW"),
            L(93, "dz", "dz-BT", "ཇོང་ཁ",             false, Tibetan),
            L(94, "bo", "bo-CN", "བོད་སྐད",            false, Tibetan),
            L(95, "kl", "kl-GL", "Kalaallisut",      false),
            L(96, "ff", "ff-NG", "Fulfulde",         false, null,      null, "ff-Latn-NG", "ff-Latn-SN"),
            L(97, "ti", "ti-ET", "ትግርኛ",             false),
            L(98, "so", "so-SO", "Soomaali",         false),
            L(99, "wo", "wo-SN", "Wolof",            false),
        };

        private static readonly Dictionary<string, LanguageInfo> ByCode =
            All.ToDictionary(l => l.Code, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<int, LanguageInfo> ById = All.ToDictionary(l => l.Id);

        private static LanguageInfo L(int id, string code, string region, string nativeName, bool rtl,
            char? digitZero = null, Dictionary<char, char> chars = null, params string[] extraCultures)
        {
            var cultures = new List<string> { region };
            cultures.AddRange(extraCultures ?? new string[0]);
            cultures.Add(code);

            // خط‌های هندی و جنوب‌شرق آسیا (همه رقم‌های بومی غیرعربی) و سینهالی به ZWNJ/ZWJ حساس‌اند
            bool preserveJoiners = (digitZero.HasValue && digitZero != ArabicIndic && digitZero != ExtendedArabicIndic)
                                   || code == "si";

            return new LanguageInfo(id, code, region, nativeName, rtl, digitZero,
                cultures.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(), chars ?? NoChars, preserveJoiners);
        }

        /// <summary>
        /// همه زبان‌های پشتیبانی‌شده به ترتیب شناسه
        /// </summary>
        public static IReadOnlyList<LanguageInfo> List => All;

        /// <summary>
        /// تعداد زبان‌های پشتیبانی‌شده
        /// </summary>
        public static int Count => All.Length;

        /// <summary>
        /// یافتن زبان با کد (fa) یا کد منطقه‌ای (fa-IR / fa_IR)؛ در صورت نبود null برمی‌گرداند
        /// </summary>
        public static LanguageInfo Find(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return null;

            string code = languageCode.Trim();
            int separator = code.IndexOfAny(new[] { '-', '_' });
            if (separator > 0)
                code = code.Substring(0, separator);

            return ByCode.TryGetValue(code, out var info) ? info : null;
        }

        /// <summary>
        /// یافتن زبان با شناسه جدول (LanguageID)
        /// </summary>
        public static LanguageInfo Find(int languageId)
        {
            return ById.TryGetValue(languageId, out var info) ? info : null;
        }

        /// <summary>
        /// آیا کد زبان پشتیبانی می‌شود
        /// </summary>
        public static bool IsSupported(string languageCode) => Find(languageCode) != null;
    }
}
