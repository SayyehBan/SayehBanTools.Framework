using SayehBanTools.Framework.Validations.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Framework.Model.Entities
{
    /// <summary>
    /// کلاس مدل های پیش فرض
    /// </summary>
    public class PublicModel
    {
        /// <summary>
        /// مقادیر پایه و پیش فرض برای تمامی جداول
        /// </summary>
        public class AuditableEntity
        {
            /// <summary>
            /// امکان حذف داره یا نه
            /// </summary>
            public bool IsApproved { get; set; }

            /// <summary>
            /// نمایش یا عدم نمایش رکورد
            /// </summary>
            public bool IsVisible { get; set; }

            /// <summary>
            /// تاریخ ثبت
            /// </summary>
            public DateTime CreatedAtDate { get; set; }

            /// <summary>
            /// تاریخ بروزرسانی
            /// </summary>
            public DateTime UpdatedAtDate { get; set; }

            /// <summary>
            /// تعداد بروزرسانی
            /// </summary>
            public int UpdateCount { get; set; }

            /// <summary>
            /// شناسه IP
            /// </summary>
            public int id_IPGeoData { get; set; }

            /// <summary>
            /// شناسه کاربر ثبت کننده
            /// </summary>
            public Guid GlobalCitizenId { get; set; }

            /// <summary>
            /// ردیابی تغییرات
            /// </summary>
            [Timestamp]
            public byte[] RowVersion { get; set; }
        }
        /// <summary>
        /// این کلاس برای عملیات ثبت و ویرایش استفاده می‌شود
        /// </summary>
        public class AuditableIUTrueEntity
        {
            /// <summary>
            /// شناسه IP
            /// </summary>
            [DisplayName("شناسه IP")]
            [PositiveNumberId(ErrorMessage = "شناسه IP نمی‌تواند خالی، صفر یا منفی باشد.")]
            public int id_IPGeoData { get; set; }
            /// <summary>
            /// شناسه کاربر ثبت کننده
            /// </summary>
            [Guid(required: true, ErrorMessage = "شناسه GUID کاربر ثبت کننده خالی یا نامعتبر است.")]
            public Guid? GlobalCitizenId { get; set; }
        }
        /// <summary>
        /// این کلاس برای عملیات ثبت و ویرایش استفاده می‌شود
        /// </summary>
        public class AuditableIUFalseEntity
        {
            /// <summary>
            /// شناسه IP
            /// </summary>
            [DisplayName("شناسه IP")]
            [PositiveNumberId(ErrorMessage = "شناسه IP نمی‌تواند خالی، صفر یا منفی باشد.")]
            public int id_IPGeoData { get; set; }
            /// <summary>
            /// شناسه کاربر ثبت کننده
            /// </summary>
            [Guid(required: false, ErrorMessage = "شناسه GUID کاربر ثبت کننده نامعتبر است.")]
            public Guid? GlobalCitizenId { get; set; }
        }
        /// <summary>
        /// پارامتر شناسه
        /// </summary>
        public class ID_Delete_Int_Parameters
        {
            /// <summary>
            ///شناسه
            /// </summary>
            [DisplayName("شناسه")]
            [PositiveNumberId(ErrorMessage = "شناسه نمی‌تواند خالی، صفر یا منفی باشد.")]
            public int id { get; set; } = 0;
        }

        /// <summary>
        /// پارامتر شناسه ها
        /// </summary>
        public class IDs_Delete_Int_Parameters
        {
            /// <summary>
            ///شناسه ها
            /// </summary>
            [DisplayName("شناسه ها")]
            [PositiveNumberIdArrayAttribute(ErrorMessage = "شناسه ها نمی‌تواند خالی، صفر یا منفی باشد.")]
            public int[] ids { get; set; } = new int[0];
        }
        /// <summary>
        /// دریافت IP از شبکه
        /// </summary>
        public class IpResponse
        {
            public string ip { get; set; }
        }
        public class SaveIP
        {
            /// <summary>
            /// دریافت IPداخلی 
            /// </summary>
            public string Local_IP { get; set; }
            /// <summary>
            /// دریافت IP عمومی
            /// </summary>
            public string Public_IP { get; set; }
        }

    }
}
