using System;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// مدیریتی برای تاریخ و زمان
    /// </summary>
    public class ManageDateAndTime
    {
        /// <summary>
        /// تبدیل تاریخ همیشه به میلادی (تنها بخش تاریخ yyyy-MM-dd)
        /// </summary>
        public class DateOnlyJsonConverter : JsonConverter<DateTime>
        {
            private const string Format = "yyyy-MM-dd";

            /// <summary>
            /// خواندن فقط تاریخ از JSON
            /// </summary>
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                    return DateTime.MinValue;

                return DateTime.ParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            /// <summary>
            /// نوشتن فقط تاریخ در JSON
            /// </summary>
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// هندل فقط تاریخ برای Dapper
        /// </summary>
        public class DateOnlyTypeHandler : Dapper.SqlMapper.TypeHandler<DateTime>
        {
            /// <summary>
            /// قرار دادن مقدار تاریخ در پارامتر دیتابیس
            /// </summary>
            public override void SetValue(IDbDataParameter parameter, DateTime value)
            {
                // فقط بخش تاریخ را ذخیره می‌کند (زمان صفر می‌شود)
                parameter.Value = value.Date;
            }

            /// <summary>
            /// تبدیل مقدار دیتابیس به DateTime (فقط تاریخ)
            /// </summary>
            public override DateTime Parse(object value)
            {
                if (value is DateTime dt)
                    return dt.Date;

                if (DateTime.TryParse(value?.ToString(), out var parsedDate))
                    return parsedDate.Date;

                return Convert.ToDateTime(value).Date;
            }
        }
    }
}