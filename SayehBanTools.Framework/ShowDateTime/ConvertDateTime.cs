using System;
using System.Globalization;
using System.Text;
namespace SayehBanTools.Framework.ShowDateTime
{
    /// <summary>
    /// این کلاس برای تبدیل تاریخ به فارسی استفاده میشود
    /// </summary>
    public class ConvertDateTime
    {    /// <summary>
         /// کد مربوط به تاریخ به صورت ماه و روز
         /// </summary>
         /// <param name="_date"></param>
         /// <returns></returns>
        public static string miladi2shamsi(DateTime _date)
        {
            PersianCalendar pc = new PersianCalendar();

            StringBuilder sb = new StringBuilder();

            sb.Append(pc.GetYear(_date).ToString("0000"));

            sb.Append("/");

            sb.Append(pc.GetMonth(_date).ToString("00"));

            sb.Append("/");

            sb.Append(pc.GetDayOfMonth(_date).ToString("00"));

            sb.Append(" امروز :");

            //sb.Append(pc.GetDayOfWeek(_date).ToString());

            string s = pc.GetDayOfWeek(_date).ToString();

            switch (s.ToUpper())
            {

                case "SATURDAY":

                    sb.Append(" شنبه");

                    break;

                case "SUNDAY":

                    sb.Append(" يكشنبه");

                    break;

                case "MONDAY":

                    sb.Append(" دوشنبه");

                    break;
                case "TUESDAY":
                    sb.Append(" سه شنبه");
                    break;

                case "WEDNESDAY":
                    sb.Append(" چهار شنبه");
                    break;
                case "THURSDAY":
                    sb.Append(" بنچ شنبه");
                    break;
                case "FRIDAY":

                    sb.Append(" جمعه");

                    break;

            }

            return sb.ToString();
        }
        /// <summary>
        /// تبدیل تاریخ و زمان UTC به تاریخ و زمان کشور مورد نظر
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertToLocalDateTime(DateTime utcDateTime)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localTimeZone);
            return localDateTime;
        }
        /*
         طریقه استفاده از دستور بالا
        DateTimeConverter converter = new DateTimeConverter();
        DateTime localDateTime = converter.ConvertToLocalDateTime(DateTime.Parse("2023-12-12 12:05:44.650"));
        Console.WriteLine(localDateTime);
        Console.ReadLine();
         */
        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// </summary>
        /// <param name="MiladiDate"></param>
        /// <returns></returns>
        public static string MiladiToShamsi(DateTime MiladiDate)
        {
            string shDate = string.Empty;
            System.Globalization.PersianCalendar perCal = new System.Globalization.PersianCalendar();

            shDate = perCal.GetYear(MiladiDate) + "/" + perCal.GetMonth(MiladiDate).ToString("00") + "/" + perCal.GetDayOfMonth(MiladiDate).ToString("00");
            //shDate = shDate + " " + MiladiDate.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            return shDate;

        }
        /// <summary>
        /// تبدیل تاریخ شمسی به میلادی
        /// </summary>
        /// <param name="ShDate"></param>
        /// <returns></returns>
        public static DateTime ShamsiToMiladi(string ShDate)
        {
            DateTime dt = default(DateTime);
            if (!string.IsNullOrEmpty(ShDate))
            {
                string[] DatePara = ShDate.Split('/');
                if (DatePara.Length == 3) // Ensure the split resulted in exactly 3 parts
                {
                    System.Globalization.PersianCalendar perCal = new System.Globalization.PersianCalendar();
                    dt = perCal.ToDateTime(
                        Convert.ToInt16(DatePara[0]),
                        Convert.ToInt16(DatePara[1]),
                        Convert.ToInt16(DatePara[2]),
                        20, 30, 15, 500,
                        System.Globalization.PersianCalendar.PersianEra
                    );
                }
            }
            return dt;
        }
    }
}
