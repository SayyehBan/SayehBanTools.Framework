using System.Collections.Generic;

namespace SayehBanTools.Framework.Translation.Google.Model
{
    /// <summary>
    /// مدل برای درخواست ترجمه
    /// </summary>
    public class TranslationRequest
    {
        /// <summary>
        /// متن اصلی
        /// </summary>
        public string OriginalText { get; set; }
        /// <summary>
        /// زبان ورودی
        /// </summary>
        public string InputLanguage { get; set; }
        /// <summary>
        /// زبان خروجی
        /// </summary>
        public string OutputLanguage { get; set; }
    }

    /// <summary>
    /// مدل برای پاسخ ترجمه
    /// </summary>
    public class TranslationResponse
    {
        /// <summary>
        /// ترجمه
        /// </summary>
        public List<string> Translations { get; set; } = new List<string>();
    }
}
