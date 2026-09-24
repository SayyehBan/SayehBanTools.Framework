namespace SayehBanTools.Framework.Model.Entities
{
    /// <summary>
    /// کلاس پیام خطا
    /// </summary>
    public class SqlModel
    {
        /// <summary>
        /// کلاس پیغام خطا برای جدول مرتبط
        /// </summary>
        public class SqlMessage
        {    /// <summary>
             /// کد خطا
             /// </summary>
            public int Code { get; set; } 
            /// <summary>
            /// پیام خطا
            /// </summary>
            public string Message { get; set; } = string.Empty;
        }
        /// <summary>
        /// کلاس پیام خطا برای جدول مرتبط
        /// </summary>
        public class SqlMessageCheckForeignKeyConstraints
        {
            /// <summary>
            /// کد خطا
            /// </summary>
            public string Message { get; set; } = string.Empty;
            /// <summary>
            /// نام جدول
            /// </summary>
            public string RelatedTable { get; set; } = string.Empty;
            /// <summary>
            /// نام ارتباط
            /// </summary>
            public string ConstraintName { get; set; } = string.Empty;
            /// <summary>
            /// تعداد رکورد
            /// </summary>
            public int SubCategoryCount { get; set; }
            /// <summary>
            /// شناسه
            /// </summary>
            public int ID { get; set; }
        }
        /// <summary>
        /// پیغام خروجی جدول
        /// </summary>
        public class OutputMessageTableType
        {
            /// <summary>
            /// کد خطا
            /// </summary>
            public int Code { get; set; }
            /// <summary>
            /// پیغام خطا
            /// </summary>
            public string Message { get; set; } = string.Empty;
            /// <summary>
            /// تعداد
            /// </summary>
            public int Count { get; set; }
            /// <summary>
            ///  شناسه ها
            /// </summary>
            public string Ids { get; set; } = string.Empty;
        }

    }
}
