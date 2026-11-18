using System;
using System.Globalization;

namespace SayehBanTools.Framework.Utilities.Helper
{
    public class GhahShomar
    {
        private string ayyam_name = string.Empty;
        private string ayyam_name2 = string.Empty;

        // دسترسی عمومی (public) برای استفاده در فرم
        public PersianCalendar MyCal { get; } = new PersianCalendar();
        public DateTime MyDate { get; }

        /// <summary>
        /// دریافت نام روز هفته (مثل: شنبه)
        /// </summary>
        public string WeekName
        {
            get
            {
                switch (MyDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday: return "شنبه";
                    case DayOfWeek.Sunday: return "یکشنبه";
                    case DayOfWeek.Monday: return "دوشنبه";
                    case DayOfWeek.Tuesday: return "سه‌شنبه";
                    case DayOfWeek.Wednesday: return "چهارشنبه";
                    case DayOfWeek.Thursday: return "پنجشنبه";
                    case DayOfWeek.Friday: return "جمعه";
                    default: return string.Empty;
                }
            }
        }

        public GhahShomar(DateTime dt)
        {
            this.MyDate = dt;
        }

        private string Ayyam()
        {
            int dayOfMonth = this.MyCal.GetDayOfMonth(this.MyDate);
            this.ayyam_name = string.Empty;

            switch (this.MyCal.GetMonth(this.MyDate))
            {
                case 1: // فروردین
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "سال نو"; break;
                        case 12: this.ayyam_name = "روز طبیعت"; break;
                        case 15: this.ayyam_name = "شهادت امیر سپهبد علی صیاد شیرازی"; break;
                        case 19: this.ayyam_name = "روز بزرگداشت عطار نیشابوری"; break;
                        case 20: this.ayyam_name = "روز ملی فناوری هسته‌ای"; break;
                    }
                    break;

                case 2: // اردیبهشت
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "روز بزرگداشت سعدی"; break;
                        case 2: this.ayyam_name = "تاسیس سپاه پاسداران انقلاب اسلامی"; break;
                        case 3: this.ayyam_name = "روز ملی کارآفرینی"; break;
                        case 5: this.ayyam_name = "شکست حمله نظامی آمریکا به ایران در طبس"; break;
                        case 9: this.ayyam_name = "روز شوراها"; break;
                        case 10: this.ayyam_name = "روز ملی خلیج فارس"; break;
                        case 12: this.ayyam_name = "شهادت استاد مرتضی مطهری"; break;
                        case 15: this.ayyam_name = "روز بزرگداشت شیخ بهایی"; break;
                        case 19: this.ayyam_name = "روز بزرگداشت فردوسی | روز بزرگداشت حکیم عمر خیام"; break;
                    }
                    break;

                case 3: // خرداد
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "روز بهره‌وری و بهینه‌سازی مصرف"; break;
                        case 3: this.ayyam_name = "روز مقاومت، ایثار و پیروزی"; break;
                        case 14: this.ayyam_name = "رحلت امام خمینی"; break;
                        case 15: this.ayyam_name = "قیام خونین ۱۵ خرداد"; break;
                        case 20: this.ayyam_name = "شهادت آیت‌الله سعیدی"; break;
                    }
                    break;

                case 4: // تیر
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "روز صنعت و معدن"; break;
                        case 7: this.ayyam_name = "شهادت آیت‌الله دکتر بهشتی"; break;
                        case 11: this.ayyam_name = "شهادت آیت‌الله صدوقی"; break;
                        case 12: this.ayyam_name = "سقوط هواپیمای مسافربری توسط آمریکا"; break;
                        case 14: this.ayyam_name = "روز قلم"; break;
                        case 19: this.ayyam_name = "روز بهزیستی و تامین اجتماعی"; break;
                    }
                    break;

                case 5: // مرداد
                    switch (dayOfMonth)
                    {
                        case 5: this.ayyam_name = "سالروز عملیات مرصاد"; break;
                        case 8: this.ayyam_name = "روز بزرگداشت شیخ شهاب‌الدین سهروردی"; break;
                        case 9: this.ayyam_name = "روز اهدای خون"; break;
                        case 14: this.ayyam_name = "صدور فرمان مشروطیت"; break;
                        case 30: this.ayyam_name = "روز بزرگداشت علامه مجلسی"; break;
                    }
                    break;

                case 6: // شهریور
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "روز پزشک"; break;
                        case 4: this.ayyam_name = "روز کارمند"; break;
                        case 8: this.ayyam_name = "روز مبارزه با تروریسم"; break;
                        case 11: this.ayyam_name = "روز صنعت چاپ"; break;
                        case 14: this.ayyam_name = "شهادت آیت‌الله قدوسی"; break;
                        case 17: this.ayyam_name = "قیام ۱۷ شهریور"; break;
                        case 20: this.ayyam_name = "شهادت آیت‌الله مدنی"; break;
                        case 27: this.ayyam_name = "روز شعر و ادب فارسی"; break;
                    }
                    break;

                case 7: // مهر
                    switch (dayOfMonth)
                    {
                        case 5: this.ayyam_name = "شکست حصر آبادان"; break;
                        case 7: this.ayyam_name = "روز آتش‌نشانی و ایمنی"; break;
                        case 8: this.ayyam_name = "روز بزرگداشت مولوی"; break;
                        case 13: this.ayyam_name = "روز نیروی انتظامی"; break;
                        case 20: this.ayyam_name = "روز بزرگداشت حافظ"; break;
                        case 21: this.ayyam_name = "روز جهانی استاندارد"; break;
                        case 26: this.ayyam_name = "روز تربیت بدنی و ورزش"; break;
                    }
                    break;

                case 8: // آبان
                    switch (dayOfMonth)
                    {
                        case 1: this.ayyam_name = "روز آمار و برنامه‌ریزی"; break;
                        case 8: this.ayyam_name = "شهادت محمدحسین فهمیده - روز نوجوان"; break;
                        case 10: this.ayyam_name = "شهادت آیت‌الله قاضی طباطبایی"; break;
                        case 13: this.ayyam_name = "روز ملی مبارزه با استکبار جهانی - روز دانش‌آموز"; break;
                        case 14: this.ayyam_name = "روز فرهنگ عمومی"; break;
                        case 24: this.ayyam_name = "روز کتاب و کتابخوانی"; break;
                    }
                    break;

                case 9: // آذر
                    switch (dayOfMonth)
                    {
                        case 5: this.ayyam_name = "روز بسیج مستضعفان"; break;
                        case 7: this.ayyam_name = "روز نیروی دریایی"; break;
                        case 9: this.ayyam_name = "روز بزرگداشت شیخ مفید"; break;
                        case 10: this.ayyam_name = "شهادت آیت‌الله سید حسن مدرس - روز مجلس"; break;
                        case 16: this.ayyam_name = "روز دانشجو"; break;
                        case 25: this.ayyam_name = "روز پژوهش"; break;
                    }
                    break;

                case 10: // دی
                    switch (dayOfMonth)
                    {
                        case 5: this.ayyam_name = "روز ملی ایمنی در برابر ززلزله"; break;
                        case 7: this.ayyam_name = "سالروز تشکیل نهضت سوادآموزی"; break;
                        case 19: this.ayyam_name = "قیام خونین مردم قم"; break;
                        case 20: this.ayyam_name = "شهادت میرزا تقی خان امیرکبیر"; break;
                        case 27: this.ayyam_name = "شهادت نواب صفوی"; break;
                    }
                    break;

                case 11: // بهمن
                    switch (dayOfMonth)
                    {
                        case 12: this.ayyam_name = "بازگشت امام خمینی به ایران - آغاز دهه فجر"; break;
                        case 19: this.ayyam_name = "روز نیروی هوایی"; break;
                        case 22: this.ayyam_name = "پیروزی انقلاب اسلامی ایران"; break;
                        case 29: this.ayyam_name = "قیام مردم تبریز"; break;
                    }
                    break;

                case 12: // اسفند
                    switch (dayOfMonth)
                    {
                        case 5: this.ayyam_name = "روز بزرگداشت خواجه نصیرالدین طوسی - روز مهندس"; break;
                        case 14: this.ayyam_name = "روز احسان و نیکوکاری"; break;
                        case 15: this.ayyam_name = "روز درختکاری"; break;
                        case 22: this.ayyam_name = "روز بزرگداشت شهدا"; break;
                        case 29: this.ayyam_name = "روز ملی شدن صنعت نفت ایران"; break;
                    }
                    break;
            }

            return string.IsNullOrEmpty(this.ayyam_name) ? "" : this.ayyam_name;
        }

        public string SimpleDate()
        {
            int year = this.MyCal.GetYear(this.MyDate);
            int month = this.MyCal.GetMonth(this.MyDate);
            int day = this.MyCal.GetDayOfMonth(this.MyDate);

            return $"{year}/{month:D2}/{day:D2}";
        }

        public string Taghvim()
        {
            return this.Ayyam();
        }

        public string time()
        {
            return DateTime.Now.Second.ToString() + " : " + DateTime.Now.Minute.ToString() + " : " + DateTime.Now.Hour.ToString();
            //throw new System.NotImplementedException();
        }
        public string GetPersianMonthName(int month)
        {
            string[] monthNames = new string[]
            {
                "", "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد",
                "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
            };
            return (month >= 1 && month <= 12) ? monthNames[month] : string.Empty;
        }
    }
}