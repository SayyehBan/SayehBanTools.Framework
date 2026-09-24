namespace SayehBanTools.Framework.Model.Entities
{
    /// <summary>
    /// دسته‌بندی‌ زبان
    /// </summary>
    public class VM_Language
    {
        /// <summary>
        /// ویو مدل کد زبانها دریافت همه
        /// </summary>
        public class LanguageCodeList
        {
            /// <summary>
            /// شناسه زبان
            /// </summary>
            public int LanguageID { get; set; }
            /// <summary>
            /// کد زبان
            /// </summary>
            public string LanguageCode { get; set; } = string.Empty;
            /// <summary>
            /// کد زبان منطقه
            /// </summary>
            public string LanguageCodeRegion { get; set; } = string.Empty;
            /// <summary>
            /// نام زبان
            /// </summary>
            public string LocalizedLanguageNames { get; set; } = string.Empty;
            /// <summary>
            ///جهت دهی زبان فارسی
            /// </summary>
            public string TextDirection { get; set; } = string.Empty;
        }
    }
}