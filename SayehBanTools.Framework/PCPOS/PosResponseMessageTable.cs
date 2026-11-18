using System;
using System.Collections.Generic;
using System.Linq;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// یک قاعده در جدول پیام پاسخ: کد پاسخ (+ در صورت نیاز کد عملیات و شرط اضافی) به پیام فارسی.
    /// برای بانک‌هایی که پیامشان به ActionCode یا طول RRN بستگی دارد (فن آوا، پرداخت نوین آرین) استفاده می‌شود.
    /// </summary>
    public class PosResponseRule
    {
        /// <summary>کد پاسخ دستگاه (Result). اگر «""» یا «"*"» باشد به‌عنوان پیش‌فرض (fallback) در نظر گرفته می‌شود.</summary>
        public string Code;
        /// <summary>کد عملیات (ActionCode)؛ خالی یعنی قاعده به آن وابسته نیست.</summary>
        public string ActionCode;
        /// <summary>شرط اضافی (مثل RRN.Length != 11)؛ نال یعنی بدون شرط.</summary>
        public Func<string, bool> ExtraCondition;
        /// <summary>پیام فارسی نتیجه</summary>
        public string Message;
    }

    /// <summary>
    /// «تونل پیام پاسخ» کارت‌خوان: تبدیل کد پاسخ هر بانک به پیام فارسی، به‌صورت داده‌محور و
    /// باز برای توسعه — بانک/کد جدید با <see cref="PosResponseMessageTable.Register"/> در شروع برنامه (یا از دیتابیس) اضافه می‌شود
    /// بدون دست زدن به کد کتابخانه. داده‌های داخلی، وفادار از کد قدیمی Aramestan (POS_Message.AlertPost) استخراج شده‌اند
    /// و اولویت و ترتیب همان کد قدیمی حفظ شده است (اولین/آخرین تطابق مثل قبل؛ «""» یعنی پیامی یافت نشد).
    /// </summary>
    public static class PosResponseMessageTable
    {
        private const int RrnLength = 11;

        private static readonly Dictionary<string, Dictionary<string, string>> Messages =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

        private static readonly Dictionary<string, List<PosResponseRule>> Rules =
            new Dictionary<string, List<PosResponseRule>>(StringComparer.Ordinal);

        private static List<PosResponseRule> RulesOf(string company)
        {
            List<PosResponseRule> list;
            if (!Rules.TryGetValue(company, out list))
            {
                list = new List<PosResponseRule>();
                Rules[company] = list;
            }
            return list;
        }

        static PosResponseMessageTable()
        {
            // ---------- داده‌های ساده (وفادار به کد قدیمی) ----------
            var sadad = Add("داده ورزی سداد");
            sadad["-1"] = "تراکنش توسط کاربر لغو شده است.\n";
            sadad["03"] = "پذيرند  کارت  شناخته شده  نیست. تراکنش بازکردن سازمان را انجام دهید.\n";
            sadad["06"] = "اشکال در سیستم مرکزی.\n";
            sadad["12"] = "پیام نادرست است.\n";
            sadad["13"] = "مقدار ریالی نادرست است.\n";
            sadad["14"] = "شماره کارت شناخته شده نیست.\n";
            sadad["25"] = "داده ای از تراکنش ارسالی نادرست است.\n";
            sadad["30"] = "فرمت اشتباه است.\n";
            sadad["33"] = "تاریخ انقضای کارت سپری شده است.\n";
            sadad["34"] = "تراکنش مالی تایید نشده است.\n";
            sadad["41"] = "کارت به علت مفقود شدن مسدود موقت است.\n";
            sadad["43"] = "کارت به علت دزديده  شدن دايما مسدود است.\n";
            sadad["51"] = "موجودی کافی نیست.\n";
            sadad["54"] = "کارت منقضي است - تراکنش لغو شد.\n";
            sadad["55"] = "رمز اشتباه می باشد.\n";
            sadad["56"] = "کارت نامعتبر است.\n";
            sadad["57"] = "تراکنش غیر مجاز.\n";
            sadad["61"] = "مبلغ تراکنش های اين کارت  از حد مجاز گذشته است.\n";
            sadad["62"] = "مبلغ تراکنش روز اين کارت  از حد مجاز گذشته است.\n";
            sadad["65"] = "تعداد تراکنش های مربوط به اين کارت  از حد مجاز گذشته است.\n";
            sadad["66"] = "حساب مسدود است.\n";
            sadad["75"] = "تعداد رمز نا معتبر از حد مجاز گذشته است.\n";
            sadad["77"] = "روز مالی صحیح نیست.\n";
            sadad["78"] = "کار فعال نیست.\n";
            sadad["79"] = "حساب تعريف نشد است.\n";
            sadad["80"] = "به دلیل اشکال در حساب تراکنش پذيرفته نیست.\n";
            sadad["81"] = "کار  قبلا باطل شد است.\n";
            sadad["83"] = "سیستم مرکزی آماده نیست.\n";
            sadad["84"] = "سیستم مرکزی فعال نیست.\n";
            sadad["85"] = "خطای درونی سیستم مرکزی.\n";
            sadad["86"] = "تراکنش روی اين دستگاه  مجاز نیست.\n";
            sadad["87"] = "خطای درونی سیستم مرکزی.\n";
            sadad["88"] = "خطای درونی سیستم مرکزی.\n";
            sadad["89"] = "خطای درونی سیستم مرکزی.\n";
            sadad["90"] = "ارتباط در حین پردازش قطع شده است.\n";
            sadad["91"] = "عدم دریافت پاسخ به علت وقوع time out در مرکز و اعلام آن توسط مرکز به دستگاه.\n";
            sadad["92"] = "صادر کننده  نا معتبر است time out در مرکز و اعلام آن توسط مرکز به دستگاه.\n";
            sadad["93"] = "به علت ترافیک تراکنشها صف تراکنشها در سیستم مرکزی پر شد است.\n";
            sadad["94"] = "شمار  تراکنش تکراری است.\n";
            sadad["96"] = "اشکال در سیستم مرکزی.\n";
            sadad["332"] = "ارتباط برقرار نمی باشد.\n";
            sadad["347"] = "ارتباط برقرار نمی باشد.\n";
            sadad["352"] = "عدم ارتباط با مرکز.\n";
            sadad["365"] = "عدم تطابق کلید ارتباطی.\n";

            var saman = Add("سامان کیش");
            saman["1"] = "پاسخ در زمان تعیین شده دریافت نشده.\n";
            saman["2"] = "تراکنش نامعتبر است.\n";
            saman["3"] = "تراکنش توسط کاربر لغو شده است.\n";
            saman["4"] = "عدم ارتباط با سرور.\n";
            saman["5"] = "عدم ارتباط با سرور.\n";
            saman["19"] = "سعی مجدد تراکنش.\n";
            saman["30"] = "ساختار داده ای نامعتبر.\n";
            saman["51"] = "عدم موجودی کافی.\n";
            saman["55"] = "رمز اشتباه است.\n";
            saman["57"] = "تراکنش برای دارنده کارت مجاز نیست.\n";
            saman["61"] = "برداشت بیش از حد مجاز.\n";
            saman["63"] = "نقض امنیتی.\n";
            saman["68"] = "پاسخ در زمان تعیین شده دریافت نشد.\n";
            saman["75"] = "تعداد دفعات ورود رمز غلط بیش از حد .\n";
            saman["78"] = "کارت غیر فعال شده است .\n";
            saman["84"] = "قطع ارتباط با صادر کننده کارت.\n";
            saman["90"] = "در حال انجام قطع ارتباط.\n";
            saman["94"] = "تراکنش تکراری.\n";
            saman["97"] = "عدم ارتباط با دستگاه پوز .\n";
            saman["100"] = "عدم موجودی کافی.\n";
            saman["101"] = "رمز اشتباه است.\n";

            var parsian = Add("پارسیان");
            parsian["50"] = "عدم ارتباط با سرور.\n";
            parsian["99"] = "تراکنش توسط کاربر لغو شد.\n";
            parsian["12"] = "تراکنش نامعتبر.\n";
            parsian["54"] = "تاریخ انقضاء کارت گذشته است.\n";
            parsian["65"] = "تعداد دفعات ورود رمز غلط بیش از حد مجاز است.\n";
            parsian["61"] = "محدودیت مبلغ تراکنش.\n";
            parsian["51"] = "موجودی کافی نیست.\n";
            parsian["55"] = "رمز نادرست است.\n";
            parsian["58"] = "ترمینال نامعتبر است.\n";

            var pasargad = Add("پاسارگاد");
            pasargad["2"] = "قبلا برگشت داده شده بود.\n";
            pasargad["3"] = "پذیرنده نامعتبر است.\n";
            pasargad["9"] = "سیستم مشغول است.\n";
            pasargad["12"] = "تراکنش نامعتبر است.\n";
            pasargad["13"] = "مبلغ نادرست است.\n";
            pasargad["14"] = "شماره کارت نامعتبر است.\n";
            pasargad["19"] = "تراکنش تکراری است.\n";
            pasargad["25"] = "رکورد پیدا نشده است.\n";
            pasargad["30"] = "داده های مورد نیاز نامعتبر است.\n";
            pasargad["31"] = "خرید نامعتبر است.\n";
            pasargad["33"] = "تاریخ انقضاء کارت به پایان رسیده است.\n";
            pasargad["34"] = "تایید نشده است.\n";
            pasargad["36"] = "کارت محدود شده است.\n";
            pasargad["38"] = "پین اشتباه بیش از حد مجاز وارد شده است.\n";
            pasargad["39"] = "اطلاعات حساب ناشناخته است.\n";
            pasargad["41"] = "کارت مفقود شده.\n";
            pasargad["43"] = "کارت دزدیده شده است.\n";
            pasargad["51"] = "عدم موجودی کافی.\n";
            pasargad["54"] = "تاریخ انقضاء کارت به پایان رسیده است.\n";
            pasargad["55"] = "پین نامعتبر.\n";
            pasargad["56"] = "کارت ثبت نشده است.\n";
            pasargad["57"] = "اجازه سرویس به کارت داده نشده است.\n";
            pasargad["58"] = "کارت در ترمینال مجاز نیست.\n";
            pasargad["61"] = "محدودیت برداشت مبلغ.\n";
            pasargad["62"] = "کارت محدود شده است.\n";
            pasargad["65"] = "Restricted Withdrawal No.\n";
            pasargad["66"] = "حساب غیر فعال است\n";
            pasargad["67"] = "کارت ضبط شده است\n";
            pasargad["68"] = "دریافت در زمان تعیین شده انجام نشد\n";
            pasargad["75"] = "سعی مجدد در ارسال رمز\n";
            pasargad["76"] = "مبلغ صحیح نیست\n";
            pasargad["77"] = "روز کاری نامعتبر است\n";
            pasargad["78"] = "کارت غیر فعال است\n";
            pasargad["79"] = "حساب تعریف نشده است\n";
            pasargad["80"] = "خطایی داخلی\n";
            pasargad["84"] = "پاسخ دریافت نشد\n";
            pasargad["90"] = "Cut Off In Progress\n";
            pasargad["91"] = "پاسخ در زمان تعیین شده دریافت نشد\n";
            pasargad["92"] = "صادر کننده کارت نامعتبر است\n";
            pasargad["93"] = "تراکنش کامل نشده است\n";
            pasargad["94"] = "پیغام تکراری\n";
            pasargad["95"] = "پین قدیمی نامعتبر است\n";
            pasargad["97"] = "همه شارژ فروخته شد\n";
            pasargad["98"] = "شارژ موجود نیست\n";
            pasargad["99"] = "شناسه پرداخت نادرست است\n";
            pasargad["120"] = "مبلغ تراکنش نادرست است\n";
            pasargad["121"] = "رمز ترمینال نادرست است\n";
            pasargad["122"] = "شناسه قبض نادرست است\n";
            pasargad["123"] = "شناسه پرداخت نامعتبر است\n";
            pasargad["124"] = "داده تراکنش ناکافی است\n";
            pasargad["125"] = "کد کد رایانه ای الکترونیکی نامعتبر است\n";
            pasargad["126"] = "تراکنش یافت نشد\n";
            pasargad["127"] = "زمان خواندن کارت یا گرفتن پین گذشته است.\n";
            pasargad["128"] = "تراکنش توسط کاربر لغو شده است.\n";
            pasargad["129"] = "خطای ترمینال.\n";
            pasargad["130"] = "حساب ناشناخته است.\n";
            pasargad["131"] = "مبلغ نامعتبر.\n";
            pasargad["132"] = "پاسخ در زمان تعیین شده دریافت نشد.\n";
            pasargad["133"] = "اتصال پوز برقرار نمی باشد.\n";
            pasargad["134"] = "حساب چندگانه نامعتبر است.\n";
            pasargad["135"] = "حساب نامعتبر.\n";
            pasargad["161"] = "مبلغ کافی نیست.\n";
            pasargad["501"] = "خطای MAC.\n";
            pasargad["999"] = "لغو توسط پوز.\n";
            pasargad["1000"] = "خطای ناشناخته.\n";

            var mellat = Add("به پرداخت ملت");
            mellat["OK"] = "دستگاه آماده برای اتصال است.\n";
            mellat["FU"] = "لغو تراکنش.\n";
            mellat["FD"] = "خطای داخلی.\n";
            mellat["FA"] = "عدم ارتباط با سرور.\n";
            mellat["FF"] = "عدم دریافت پاسخ در زمان تعیین شده.\n";
            mellat["01"] = "با بانک صادره کننده کارت تماس بگیرید.\n";
            mellat["02"] = "تراکنش اصلاحی قبلا ارسال شده است.\n";
            mellat["03"] = "کارت نامعتبر است.\n";
            mellat["04"] = "از انجام تراکنش صرفنظر شده است.\n";
            mellat["05"] = "مشکل در سوئیچ کارت.\n";
            mellat["09"] = "سیستم مشغول است مجدد تلاش کنید.\n";
            mellat["12"] = "تراکنش نامعتبر.\n";
            mellat["13"] = "مبلغ نامعتبر.\n";
            mellat["14"] = "شماره کارت نامعتبر.\n";
            mellat["15"] = "صادر کننده کارت نامشخص است.\n";
            mellat["17"] = "تراکنش توسط کاربر لغو شده است.\n";
            mellat["19"] = "از انجام تراکنش صرفنظر شد ورود مجدد تراکنش.\n";
            mellat["20"] = "کد پاسخ نامعتبر است.\n";
            mellat["21"] = "هیچ پردازشی صورت نگرفت.\n";
            mellat["22"] = "خطای سیستم.\n";
            mellat["25"] = "تراکنش صحیح پیدا نشد.\n";
            mellat["30"] = "ساختار پیغام نامعتبر است.\n";
            mellat["31"] = "کارت پذیرنده نامعتبر است.\n";
            mellat["32"] = "ثبت تراکنش صحیح.\n";
            mellat["33"] = "تاریخ انقضاء کارت سپری شده است..\n";
            mellat["34"] = "ثبت تراکنش صرفنظر شده.\n";
            mellat["36"] = "تراکنش انجام نشد به دلیل اینکه کارت محدود شده است.\n";
            mellat["38"] = "تعداد دفعات ورود رمز غلط بیش از حد مجاز است.\n";
            mellat["39"] = "شماره حساب کارت ناشناخته است.\n";
            mellat["40"] = "درخواست ناشناخته.\n";
            mellat["41"] = "کارت مفقود شده یا مسدود موقت است.\n";
            mellat["43"] = "کارت مسروقه میباشد یا مسدود دایم است.\n";
            mellat["51"] = "موجودی حساب کافی نیست.\n";
            mellat["53"] = "خطا در عملیات حسابداری.\n";
            mellat["54"] = "تاریخ استفاده از کارت به پایان رسیده است.\n";
            mellat["55"] = "رمز نامعتبر میباشد.\n";
            mellat["56"] = "شکاره کارت اشکال دارد.\n";
            mellat["57"] = "دارنده کارت مجاز به استفاده از کارت نیست.\n";
            mellat["58"] = "پذیرنده کارت مجاز به استفاده از این تراکنش نیست.\n";
            mellat["61"] = "مبلغ تراکنش از حد مجاز بالاتر است.\n";
            mellat["62"] = "کارت محدود شده است.\n";
            mellat["64"] = "از انجام تراکنش صرفنظر شد،عدم اعتبار مبلغ تراکنش اصلاحیه.\n";
            mellat["65"] = "تعداد دفعات تراکنش از حد مجاز بیشتر است.\n";
            mellat["66"] = "شماره حساب فعال نیست.\n";
            mellat["68"] = "به علت تاخیر پاسخی دریافت نشد.\n";
            mellat["75"] = "از انجام تراکنش صرفنظر شد به علت ورود بیش از حد رمز اشتباه.\n";
            mellat["76"] = "شماره کارت در سیستم موجود نیست.\n";
            mellat["77"] = "از انجام تراکنش صرفنظر شد تاریخ روز کاری نامعتبر است.\n";
            mellat["78"] = "کارت فعال نیست.\n";
            mellat["79"] = "حساب تعریف نشده یا حساب متصل به کارت فعال نیست.\n";
            mellat["80"] = "تراکنش موفق عمل نکرده است.\n";
            mellat["84"] = "صادر کننده وضعیت عملیاتی نیست.\n";
            mellat["88"] = "سیستم به طور موقت دچار اشکال شده است.\n";
            mellat["90"] = "سیستم صدور مجوز غیرفعال است.\n";
            mellat["91"] = "ارتباط به طور موقت قطع می باشد.\n";
            mellat["92"] = "صادر کننده کارت نامعتبر است.\n";
            mellat["93"] = "تراکنش به طور کامل انجام نشده است.\n";
            mellat["94"] = "عدم انطباق فایل های پایانه و مرکز.\n";
            mellat["95"] = "ترمینال غیر فعال است.\n";
            mellat["96"] = "پایانه مجاز به انجام تراکنش نیست.\n";
            mellat["97"] = "عدم انطباق شماره دسته گردش.\n";
            mellat["98"] = "شماره دسته گردش موجود نیست.\n";
            mellat["99"] = "ترمینال تعریف نشده است.\n";

            var irankish = Add("ایران کیش");
            irankish["1"] = "تراکنش توسط کاربر لغو شده است.\n";
            irankish["13"] = "مبلغ نامعتبر است.\n";
            irankish["51"] = "موجودی کافی نیست.\n";
            irankish["55"] = "رمز اشتباه است.\n";
            irankish["100"] = "داده نامعتبر است.\n";
            irankish["200"] = "تراکنش نامعتبر است.\n";
            irankish["201"] = "سریال نامعتبر است.\n";
            irankish["202"] = "ترمینال نامعتبر است.\n";
            irankish["203"] = "پذیرنده نامعتبر است.\n";
            irankish["998"] = "عدم دریافت پاسخ در زمان تعیین شده.\n";
            irankish["-999"] = "خطا نامشخص.\n";

            var sayan = Add("سایان کارت");
            sayan["51"] = "عدم موجودی کافی.\n";
            sayan["55"] = "رمز نامعتبر است.\n";
            sayan["100"] = "امکان برقراری ارتباط وجود ندارد.\n";
            sayan["103"] = "لغو عملیات تراکنش توسط کاربر.\n";
            sayan["104"] = "کارت معتبر نمی باشد یا درست خوانده نشده است.\n";

            var asan = Add("آسان پرداخت");
            asan["51"] = "موجودی کافی نیست.\n";
            asan["54"] = "تاریخ انقضاء کارت سپری شده است.\n";
            asan["55"] = "رمز اشتباه است.\n";
            asan["110"] = "مبلغ وارد شده اشتباه است.\n";
            asan["600"] = "تراکنش توسط کاربر لغو شده است.\n";
            asan["603"] = "کابل ارتباطی دستگاه با سوئیچ بانک قطع است.\n";
            asan["607"] = "عملیات ناموفق حرید زیر لغو شد.\n";

            var mabna = Add("مبنا آریا");
            mabna["1"] = "صادر کننده ی کارت از انجام تراکنش صرف نظر کرد.\n";
            mabna["3"] = "پذیرنده فروشگاهی نامعتبر است.\n";
            mabna["4"] = "کارت توسط دستگاه ضبط شود.\n";
            mabna["5"] = "از انجام تراکنش صرف نظر شد.\n";
            mabna["6"] = "خطا مربوط به سیستم سوئیچ کارت.\n";
            mabna["7"] = "به دلیل شرایط خاص کارت توسط دستگاه ضبط شود.\n";
            mabna["8"] = "با تشخیص هویت دارنده کارت، تراکنش موفق میباشد.\n";
            mabna["12"] = "تراکنش نامعتبر.\n";
            mabna["14"] = "شماره کارت ارسالی نامعتبر است. وجود ندارد.\n";
            mabna["15"] = "صادر کننده ی کارت نامعتبر است. وجود ندارد.\n";
            mabna["16"] = "تراکنش مورد تایید است و اطلاعات شیار سوم کارت بروزرسانی شود.\n";
            mabna["19"] = "تراکنش مجدد ارسال شود.\n";
            mabna["23"] = "کارمزد ارسالی پذیرنده غیر قابل قبول است.\n";
            mabna["30"] = "قالب پیام دارای اشکال است.\n";
            mabna["31"] = "پذیرنده توسط سوئیچ پشتیبانی نمیشود.\n";
            mabna["33"] = "تاریخ انقضاء کارت سپری شده است.\n";
            mabna["34"] = "کارت مظنون به تقلب است، کارت توسط دستگاه ضبط شود.\n";
            mabna["36"] = "کارت محدود شده است. کارت توسط دستگاه ضبط شود.\n";
            mabna["38"] = "تعداد دفعات ورود رمز غلط بیش از حد مجاز است.\n";
            mabna["39"] = "کارت حساب اعتباری ندارد.\n";
            mabna["40"] = "عملیات درخواستی پشتیبانی نمی گردد.\n";
            mabna["41"] = "عملیات درخواستی پشتیبانی نمی گردد.\n";
            mabna["42"] = "کارت حساب عمومی ندارد.\n";
            mabna["43"] = "کارت مسروقه میباشد. کارت توسط دستگاه ضبط شود.\n";
            mabna["44"] = "کارت حساب سرمایه گزاری ندارد.\n";
            mabna["51"] = "دستگاه آماده کار نمی باشد- اتمام کاغذ یا قطع ارتباط.\n";
            mabna["52"] = "کارت حساب جاری ندارد.\n";
            mabna["53"] = "کارت حساب قرض الحسنه ندارد.\n";
            mabna["54"] = "تاریخ انقضاء کارت سپری شده است.\n";
            mabna["55"] = "رمز کارت نامعتبر است.\n";
            mabna["56"] = "شماره کارت دارای اشکال است.\n";
            mabna["57"] = "انجام تراکنش مربوط توسط پایانه ی انجام دهنده مجاز نمی باشد.\n";
            mabna["58"] = "انجام تراکنش مربوط توسط پایانه ی انجام دهنده مجاز نمی باشد.\n";
            mabna["59"] = "کارت مظنون به تقلب است.\n";
            mabna["61"] = "مبلغ تراکنش بیش از حد مجاز است.\n";
            mabna["62"] = "کارت محدود شده است.\n";
            mabna["63"] = "تمهیدات امنیتی نقض گردیده است.\n";
            mabna["65"] = "تعداد درخواست تراکنش بیش از حد مجاز میباشد.\n";
            mabna["67"] = "کارت توسط دستگاه ضبط شود.\n";
            mabna["75"] = "تعداد دفعات ورود رمز غلط بیش از حد مجاز است.\n";
            mabna["77"] = "روز مالی تراکنش نامعتبر است.\n";
            mabna["78"] = "کارت فعال نیست.\n";
            mabna["79"] = "حساب متصل به کارت نامعتبر است یا دارای اشکال است.\n";
            mabna["80"] = "تراکنش موفق عمل نکرده است.\n";
            mabna["83"] = "ارائه دهنده خدمات پرداخت یا سامانه شاپرک اعلام Sign Off نموده است.\n";
            mabna["84"] = "وضعیت سامانه یا بانک مقصد تراکنش غیر فعال میباشد.\n";
            mabna["86"] = "موسسه ارسال، شاپرک یا مقصد تراکنس در حالت Sign Off است.\n";
            mabna["90"] = "سامانه مقصد تراکنش در حال انجام عملیات پایان روز میباشد.\n";
            mabna["91"] = "صادر کننده یا سوئیچ مقصد فعال نمی باشد.\n";
            mabna["92"] = "مسیر برای ارسال تراکنش به مقصد یافت نشد.\n";
            mabna["93"] = "تراکنش با موفقیت انجام نشد.(کمبود منابع و نقص موارد قانونی).\n";
            mabna["94"] = "ارسال تراکنش تکراری).\n";
            mabna["96"] = "بروز خطایی سیستمی در انجام تراکنش).\n";
            mabna["97"] = "فرایند تغییر کلید برای صادر کننده یا پذیرنده در حال انجام است).\n";
            mabna["98"] = "خطای سوئیچ.\n";
            mabna["99"] = "خطا ناشناخته\n";
            mabna["01"] = "خطای ارتباطی).\n";
            mabna["02"] = "خطای در دریافت).\n";
            mabna["03"] = "خطای در ارسال).\n";
            mabna["04"] = "پوز تراکنش معکوس دارد).\n";
            mabna["05"] = "لغو توسط کاربر).\n";
            mabna["06"] = "خط مشغول است).\n";
            mabna["07"] = "پاسخی دریافت نشد)).\n";
            mabna["08"] = "No career.\n";
            mabna["09"] = "No dial tone.\n";
            mabna["10"] = "خطای داخلی.\n";
            mabna["11"] = "پاسخی در زمان تعیین شده دریافت نشد.\n";
            mabna["50"] = "شماره ترمینال یا شماره سریال نادرست است.\n";
            // توجه: کد «51» مبنا آریا در کد قدیمی دو بار نوشته شده و چون بعدی بعداً اجرا می‌شود،
            // معنای دوم («دستگاه آماده کار نمی باشد») در دیکشنری نگه داشته شده است (وفادار به رفتار واقعی).

            // ---------- فن آوا: قواعد شرط‌دار + زنجیره else-if ----------
            var f = RulesOf("فن آوا");
            f.Add(new PosResponseRule { Code = "0", ActionCode = "0", ExtraCondition = delegate(string rrn) { return rrn.Length != RrnLength; }, Message = "عدم ارتباط با سرور.\n" });
            f.Add(new PosResponseRule { Code = "3", ActionCode = "0", Message = "تراکنش قبل از کشیدن کارت لغو شده است.\n" });
            f.Add(new PosResponseRule { Code = "4", ActionCode = "0", Message = "خطا در خواندن کارت.\n" });
            f.Add(new PosResponseRule { Code = "1", ActionCode = "0", ExtraCondition = delegate(string rrn) { return rrn.Length != RrnLength; }, Message = "تراکنش بعد از کشیدن کارت لغو شده است.\n" });
            f.Add(new PosResponseRule { Code = "227", ActionCode = "4", Message = "تراکنش تکراری.\n" });
            f.Add(new PosResponseRule { Code = "203", ActionCode = "4", Message = "تاریخ انقضاء کارت سپرده شده است..\n" });
            f.Add(new PosResponseRule { Code = "226", ActionCode = "4", Message = "نقض امنیتی.\n" });
            f.Add(new PosResponseRule { Code = "234", ActionCode = "4", Message = "کارت غیر فعال شده است.\n" });
            f.Add(new PosResponseRule { Code = "231", ActionCode = "4", Message = "تعداد دفعات ورود رمز غلط بیش از حد مجاز است.\n" });
            f.Add(new PosResponseRule { Code = "182", ActionCode = "4", Message = "مبلغ تصحیح شده معتبر نیست.\n" });
            f.Add(new PosResponseRule { Code = "224", ActionCode = "7", Message = "پاسخ در زمان تعیین شده دریافت نشد.\n" });
            f.Add(new PosResponseRule { Code = "208", ActionCode = "4", Message = "برداشت بیش از مجاز.\n" });
            f.Add(new PosResponseRule { Code = "202", ActionCode = "4", Message = "عدم موجودی کافی.\n" });
            f.Add(new PosResponseRule { Code = "101", ActionCode = "4", Message = "رمز نامعتبر است.\n" });
            // زنجیره else-if قدیمی (اولین تطابق برنده است؛ چون کدها یکتا هستند، ترتیب حفظ شده)
            f.Add(new PosResponseRule { Code = "00", Message = "تراکنش به درستی انجام شده است.\n" });
            f.Add(new PosResponseRule { Code = "02", Message = "تراکنش قبلاً REVERSE شده است (در پاسخ به (REVERSE\n" });
            f.Add(new PosResponseRule { Code = "03", Message = "پذیرنده کارت شناخته شده نیست. باید عمل بازکردن فروشگاه را انجام دهید.\n" });
            f.Add(new PosResponseRule { Code = "06", Message = "اشکال در سیستم مرکز\n" });
            f.Add(new PosResponseRule { Code = "12", Message = "پیام نادرست است. یکی از دلایل این خطا عدم مجوز پایانه در انجام عملیات مورد نظر می باشد.\n" });
            f.Add(new PosResponseRule { Code = "13", Message = "مقدار ریالی غلط است. (مبلغ تراکنش صحیح نیست)\n" });
            f.Add(new PosResponseRule { Code = "14", Message = "شماره کارت شناخته شده نیست. (کارت در سیستم خدمات کارت نامعتبر است)\n" });
            f.Add(new PosResponseRule { Code = "15", Message = "در پاسخ به تراکنش باز کردن فروشگاه: قبلاً باز شده است\n" });
            f.Add(new PosResponseRule { Code = "16", Message = "در پاسخ به تراکنش بستن فروشگاه: قبلاً بسته شده است.\n" });
            f.Add(new PosResponseRule { Code = "19", Message = "در پاسخ به تراکنش تراز تجاری پایانه: مجموع تراکنشهای پایانه با مجموع تراکنشهای مرکز برابر نیست.\n" });
            f.Add(new PosResponseRule { Code = "20", Message = "پاسخ دریافت شده از ATM ناشناخته است.\n" });
            f.Add(new PosResponseRule { Code = "21", Message = "هیچ عملی بر روی تراکنش انجام نشد.\n" });
            f.Add(new PosResponseRule { Code = "22", Message = "خطای سخت افزاری در ATM\n" });
            f.Add(new PosResponseRule { Code = "24", Message = "در پاسخ به تراکنش پایان روز: عملیات انجام نشد. باید تراکنش دوباره تکرار گردد.\n" });
            f.Add(new PosResponseRule { Code = "25", Message = "داده ارسالی غلط است واین داده در مرکز پیدا نشد. (ممکن است شماره پایانه باشد)\n" });
            f.Add(new PosResponseRule { Code = "29", Message = "در پاسخ به تراکنش پایان روز انجام نشده. لازم است پیگیری شود.\n" });
            f.Add(new PosResponseRule { Code = "30", Message = "فرمت اشتباه است.\n" });
            f.Add(new PosResponseRule { Code = "33", Message = "تاریخ انقضای کارت سیری شده است.\n" });
            f.Add(new PosResponseRule { Code = "34", Message = "تراکنش مالی تایید نشده است (در پاسخ به (REVERSE)\n" });
            f.Add(new PosResponseRule { Code = "37", Message = "کارت مقصد در تراکنش انتقال مشکل دارد.\n" });
            f.Add(new PosResponseRule { Code = "39", Message = "حساب اعتباری وجود ندارد.\n" });
            f.Add(new PosResponseRule { Code = "41", Message = "کارت به علت مفقود شدن مسدود موقت است.\n" });
            f.Add(new PosResponseRule { Code = "43", Message = "کارت به علت دزدیده شدن دائماً مسدود است.\n" });
            f.Add(new PosResponseRule { Code = "44", Message = "اطلاعات قبض صحیح نیست یا قبض وجود ندارد.\n" });
            f.Add(new PosResponseRule { Code = "46", Message = "بار کد صحیح نیست.\n" });
            f.Add(new PosResponseRule { Code = "47", Message = "قبض منقضی شده است.\n" });
            f.Add(new PosResponseRule { Code = "48", Message = "قبض قبلاً پرداخت شده است.\n" });
            f.Add(new PosResponseRule { Code = "50", Message = "لغو خرید پس از زمان مجاز صورت میگردد.\n" });
            f.Add(new PosResponseRule { Code = "51", Message = "کمبود وجه (جهت انجام تراکنش مالی)\n" });
            f.Add(new PosResponseRule { Code = "53", Message = "خطای امنیتی\n" });
            f.Add(new PosResponseRule { Code = "55", Message = "رمز نامعتبر\n" });
            f.Add(new PosResponseRule { Code = "56", Message = "کارت نامعتبر (کارت تعریف نشده یا داده کارت غلط است)\n" });
            f.Add(new PosResponseRule { Code = "57", Message = "تراکنش غیر مجاز\n" });
            f.Add(new PosResponseRule { Code = "58", Message = "ترمینال اجازه عملیات مورد نظر را ندارد.\n" });
            f.Add(new PosResponseRule { Code = "59", Message = "مشکوک به تقلب\n" });
            f.Add(new PosResponseRule { Code = "60", Message = "در پاسخ به تراکنش لیست سیاه یا پایان روز لیست در مرکز حاضر نیست\n" });
            f.Add(new PosResponseRule { Code = "61", Message = "مبلغ تراکنشهای مربوط به به این کارت از حد مجاز گذشته است\n" });
            f.Add(new PosResponseRule { Code = "62", Message = "مبلغ تراکنشهای روز مربوط به به این کارت از حد مجاز گذشته است\n" });
            f.Add(new PosResponseRule { Code = "63", Message = "MAC صحیح نمی باشد\n" });
            f.Add(new PosResponseRule { Code = "64", Message = "در پاسخ به تراکنش پایان روز تراکنش ارسال شده صحیح نیست.\n" });
            f.Add(new PosResponseRule { Code = "65", Message = "تعداد تراکنش های مربوط به این کارت از حد مجاز گذشته است.\n" });
            f.Add(new PosResponseRule { Code = "66", Message = "حساب مسدود است\n" });
            f.Add(new PosResponseRule { Code = "67", Message = "کارت ضبط شود\n" });
            f.Add(new PosResponseRule { Code = "68", Message = "مانده کارت صحیح نیست\n" });
            f.Add(new PosResponseRule { Code = "70", Message = "در پاسخ به تراکنش پایان روز تراز صحیح نیست و لیست سیاه در مرکز حاضر نیست\n" });
            f.Add(new PosResponseRule { Code = "71", Message = "در پاسخ به تراکنش پایان روز تراکنش ارسال شده صحیح نیست ولیست سیاه حاضر نیست\n" });
            f.Add(new PosResponseRule { Code = "74", Message = "بدلیل یکی بودن شماره حساب مبدا و مقصد تراکنش انجام نمی گردد\n" });
            f.Add(new PosResponseRule { Code = "75", Message = "تعدادرمز نامعتبر از حد مجاز گذشته است\n" });
            f.Add(new PosResponseRule { Code = "77", Message = "روز مالی صحیح نیست (در تراکنش شتابی)\n" });
            f.Add(new PosResponseRule { Code = "78", Message = "کارت فعال نیست\n" });
            f.Add(new PosResponseRule { Code = "79", Message = "حساب تعریف نشده است\n" });
            f.Add(new PosResponseRule { Code = "80", Message = "به دلیل اشکال در حساب تراکنش پذیرفته نیست\n" });
            f.Add(new PosResponseRule { Code = "81", Message = "کارت قبلاً باطل شده است\n" });
            f.Add(new PosResponseRule { Code = "82", Message = "پاسخ ACK از ATM دریافت نشد\n" });
            f.Add(new PosResponseRule { Code = "83", Message = "سیستم مرکزی آماده نیست\n" });
            f.Add(new PosResponseRule { Code = "84", Message = "سیستم مرکزی فعال نیست\n" });
            f.Add(new PosResponseRule { Code = "85", Message = "خطای درونی سیستم مرکزی\n" });
            f.Add(new PosResponseRule { Code = "86", Message = "تراکنش روی این دستگاه مجاز نیست\n" });
            f.Add(new PosResponseRule { Code = "87", Message = "خطای درونی سیستم مرکزی\n" });
            f.Add(new PosResponseRule { Code = "88", Message = "خطای درونی سیستم مرکزی\n" });
            f.Add(new PosResponseRule { Code = "89", Message = "خطای درونی سیستم مرکزی\n" });
            f.Add(new PosResponseRule { Code = "90", Message = "ارتباط در حین پردازش قطع شده است\n" });
            f.Add(new PosResponseRule { Code = "91", Message = "عدم دریافت پاسخ به علت وقوع TIMEOUT در مرکزو اعلام آن به دستگاه\n" });
            f.Add(new PosResponseRule { Code = "92", Message = "صادر کننده نامعتبر است\n" });
            f.Add(new PosResponseRule { Code = "93", Message = "به علت تراکنش ها صف تراکنشها در سیستم پر شده است\n" });
            f.Add(new PosResponseRule { Code = "94", Message = "شماره تراکنش تکراری است\n" });
            f.Add(new PosResponseRule { Code = "96", Message = "اشکال در سیستم مرکز\n" });
            // elseِ نهایی زنجیره: هر کد دیگری → خطای ناشناخته (همان معنای کد قدیمی)
            f.Add(new PosResponseRule { Code = "*", Message = "خطای ناشناخته.\n" });

            // ---------- پرداخت نوین آرین: همه پیام‌ها وابسته به ActionCode ----------
            var n = RulesOf("پرداخت نوین آرین");
            n.Add(new PosResponseRule { Code = "1", ActionCode = "2", Message = "به صادر کننده کارت رجوع شود - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "2", ActionCode = "2", Message = "با توجه به شرايط خاص به وجود آمده به صادر کننده کارت رجوع شود.\n" });
            n.Add(new PosResponseRule { Code = "3", ActionCode = "2", Message = "پذيرنده فروشگاه هي معتبر نيست.\n" });
            n.Add(new PosResponseRule { Code = "4", ActionCode = "2", Message = "ضبط کارت - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "5", ActionCode = "2", Message = "مجوز صادر نشود.\n" });
            n.Add(new PosResponseRule { Code = "6", ActionCode = "2", Message = "خطا.\n" });
            n.Add(new PosResponseRule { Code = "7", ActionCode = "2", Message = "ضبط کارت در شرايط خاص - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "8", ActionCode = "2", Message = "مجوز پس از شناسايي صادر شود - تراکنش انجام شد.\n" });
            n.Add(new PosResponseRule { Code = "9", ActionCode = "2", Message = "صادرکننده کارت نامعتبر است.\n" });
            n.Add(new PosResponseRule { Code = "10", ActionCode = "2", Message = "تراکنش با جزئي از مبلغ انجام شد - تراکنش انجام شد.\n" });
            n.Add(new PosResponseRule { Code = "11", ActionCode = "2", Message = "تراکنش جهت اشخاص خاص انجام شد.\n" });
            n.Add(new PosResponseRule { Code = "12", ActionCode = "2", Message = "تراکنش معتبر نيست.\n" });
            n.Add(new PosResponseRule { Code = "13", ActionCode = "2", Message = "مبلغ معتبر نيست.\n" });
            n.Add(new PosResponseRule { Code = "14", ActionCode = "2", Message = "شماره کارت معتبر نيست.\n" });
            n.Add(new PosResponseRule { Code = "15", ActionCode = "2", Message = "چنين صادر کننده اي وجود ندارد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "16", ActionCode = "2", Message = "تراکنش پس از بروز رساني کارت انجام شد - تراکنش انجام شد.\n" });
            n.Add(new PosResponseRule { Code = "17", ActionCode = "2", Message = "انصراف مشتري.\n" });
            n.Add(new PosResponseRule { Code = "18", ActionCode = "2", Message = "اعتراض مشتري.\n" });
            n.Add(new PosResponseRule { Code = "19", ActionCode = "2", Message = "تراکنش تکراري - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "20", ActionCode = "2", Message = "پاسخ معتبر نيست.\n" });
            n.Add(new PosResponseRule { Code = "21", ActionCode = "2", Message = "عمليات خاصي انجام نشد.\n" });
            n.Add(new PosResponseRule { Code = "22", ActionCode = "2", Message = "احتمال خرابي.\n" });
            n.Add(new PosResponseRule { Code = "23", ActionCode = "2", Message = "کارمزد تراکنش غير قابل اعمال است - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "24", ActionCode = "2", Message = "دريافت کننده امکان پشتيباني از عمليات فايلي را ندارد.\n" });
            n.Add(new PosResponseRule { Code = "25", ActionCode = "2", Message = "امکان يافتن رکورد در فايل موجود نيست.\n" });
            n.Add(new PosResponseRule { Code = "26", ActionCode = "2", Message = "عمليات فايلي مربوط به رکورد تکراري است. رکورد قبلي جايگزين مي شود.\n" });
            n.Add(new PosResponseRule { Code = "27", ActionCode = "2", Message = "عمليات فايلي تصحيح فيلد با خطا مواجه شد.\n" });
            n.Add(new PosResponseRule { Code = "28", ActionCode = "2", Message = "انجام عمليات فايلي روي يک فايل قفل شده.\n" });
            n.Add(new PosResponseRule { Code = "29", ActionCode = "2", Message = "عمليات فايلي موفقيت آميز نبود - با پذيرنده تماس بگيريد.\n" });
            n.Add(new PosResponseRule { Code = "30", ActionCode = "2", Message = "اشکال در فرمت اطلاعات.\n" });
            n.Add(new PosResponseRule { Code = "31", ActionCode = "2", Message = "بانک توسط سويچ پشتيباني نمي شود - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "32", ActionCode = "2", Message = "تراکنش به صورت جزئي انجام شد.\n" });
            n.Add(new PosResponseRule { Code = "33", ActionCode = "2", Message = "کارت منقضي شده - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "34", ActionCode = "2", Message = "احتمال سوء استفاده - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "35", ActionCode = "2", Message = "دريافت کننده کارت نمي تواند يک پذيرنده باشد - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "36", ActionCode = "2", Message = "کارت داراي محدوديت مي باشد - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "37", ActionCode = "2", Message = "تماس دريافت کننده کارت با تشکيلات امنيتي پذيرنده - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "38", ActionCode = "2", Message = "تعداد دفعات مجاز تکرار رمز مشتري به پايان رسيد - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "39", ActionCode = "2", Message = "حساب اعتباري وجود ندارد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "40", ActionCode = "2", Message = "عمليات درخواستي پشتيباني نمي شود - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "41", ActionCode = "2", Message = "کارت گم شده است - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "42", ActionCode = "2", Message = "حساب عمومي وجود ندارد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "43", ActionCode = "2", Message = "کارت مسروقه است - ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "44", ActionCode = "2", Message = "حساب سرمايه گذاري وجود ندارد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "51", ActionCode = "2", Message = "موجودي حساب کافي نيست - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "52", ActionCode = "2", Message = "حساب جاري وجود ندارد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "53", ActionCode = "2", Message = "حساب پس انداز موجود نيست - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "54", ActionCode = "2", Message = "کارت منقضي است - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "55", ActionCode = "2", Message = "رمز اشتباه است.\n" });
            n.Add(new PosResponseRule { Code = "56", ActionCode = "2", Message = "رکورد کارت وجود ندارد.\n" });
            n.Add(new PosResponseRule { Code = "57", ActionCode = "2", Message = "دارنده کارت مجاز به انجام تراکنش نيست - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "58", ActionCode = "2", Message = "پايانه مجاز به انجام تراکنش نيست - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "59", ActionCode = "2", Message = "احتمال سوء استفاده - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "60", ActionCode = "2", Message = "دريافت کننده کارت نمي تواند يک پذيرنده باشد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "61", ActionCode = "2", Message = "مبلغ از سقف برداشت نقدي بيشتر است - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "62", ActionCode = "2", Message = "کارت داراي محدوديت مي باشد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "63", ActionCode = "2", Message = "نقض موارد امنيتي.\n" });
            n.Add(new PosResponseRule { Code = "64", ActionCode = "2", Message = "مبلغ تراکنش اصلي اشتباه است - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "65", ActionCode = "2", Message = "مبلغ از سقف دوره اي برداشت نقدي بيشتر است - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "66", ActionCode = "2", Message = "تماس دريافت کننده کارت با تشکيلات امنيتي پذيرنده - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "67", ActionCode = "2", Message = "ضبط کارت.\n" });
            n.Add(new PosResponseRule { Code = "68", ActionCode = "2", Message = "پاسخ در فرصت مشخص شده دريافت نشد.\n" });
            n.Add(new PosResponseRule { Code = "75", ActionCode = "2", Message = "تعداد دفعات مجاز تکرار رمز مشتري به پايان رسيد - تراکنش لغو شد.\n" });
            n.Add(new PosResponseRule { Code = "76", ActionCode = "2", Message = "Invalid Interchange Amount.\n" });
            n.Add(new PosResponseRule { Code = "77", ActionCode = "2", Message = "Invalid Business Date.\n" });
            n.Add(new PosResponseRule { Code = "78", ActionCode = "2", Message = "Deactivated Card.\n" });
            n.Add(new PosResponseRule { Code = "79", ActionCode = "2", Message = "Invalid Account.\n" });
            n.Add(new PosResponseRule { Code = "80", ActionCode = "2", Message = "Transaction Denied.\n" });
            n.Add(new PosResponseRule { Code = "81", ActionCode = "2", Message = "Cancelled Card.\n" });
            n.Add(new PosResponseRule { Code = "82", ActionCode = "2", Message = "No Ack From ATM.\n" });
            n.Add(new PosResponseRule { Code = "83", ActionCode = "2", Message = "Host Refuse.\n" });
            n.Add(new PosResponseRule { Code = "84", ActionCode = "2", Message = "Issuer Down.\n" });
            n.Add(new PosResponseRule { Code = "85", ActionCode = "2", Message = "Invalid Originator Or Processor.\n" });
            n.Add(new PosResponseRule { Code = "86", ActionCode = "2", Message = "Not Allowed On Device.\n" });
            n.Add(new PosResponseRule { Code = "87", ActionCode = "2", Message = "Pin Key Sync Error.\n" });
            n.Add(new PosResponseRule { Code = "88", ActionCode = "2", Message = "Mac Key Sync Error.\n" });
            n.Add(new PosResponseRule { Code = "89", ActionCode = "2", Message = "External Switch Decline.\n" });
            n.Add(new PosResponseRule { Code = "90", ActionCode = "2", Message = "Cut Off Is In Progress.\n" });
            n.Add(new PosResponseRule { Code = "91", ActionCode = "2", Message = "Issuer Or Switch Is In Operative - Decline.\n" });
            n.Add(new PosResponseRule { Code = "92", ActionCode = "2", Message = "Financial Inst Or Net Not Found For Routing – Decline.\n" });
            n.Add(new PosResponseRule { Code = "93", ActionCode = "2", Message = "Transaction Cannot Be Completed.\n" });
            n.Add(new PosResponseRule { Code = "94", ActionCode = "2", Message = "Duplicate Transmission.\n" });
            n.Add(new PosResponseRule { Code = "95", ActionCode = "2", Message = "Reconcile Error.\n" });
            n.Add(new PosResponseRule { Code = "96", ActionCode = "2", Message = "System Malfunction.\n" });
            n.Add(new PosResponseRule { Code = "99", ActionCode = "1", Message = "تراکنش توسط کاربر لغو شده است.\n" });
            n.Add(new PosResponseRule { Code = "92", ActionCode = "1", Message = "خطاي عمومي.\n" });
            n.Add(new PosResponseRule { Code = "1", ActionCode = "1", Message = "کارت درست خوانده نشده است.\n" });
            n.Add(new PosResponseRule { Code = "30", ActionCode = "1", Message = "خطا در فرمت داده ارسالي.\n" });
        }

        private static Dictionary<string, string> Add(string company)
        {
            var table = new Dictionary<string, string>(StringComparer.Ordinal);
            Messages[company] = table;
            return table;
        }

        /// <summary>نام‌های نمایشی بانک‌های موجود در جدول (برای رابط کاربری یا اعتبارسنجی رجیستری توسعه‌ها)</summary>
        public static IReadOnlyCollection<string> KnownCompanies
        {
            get { return Messages.Keys.Concat(Rules.Keys).Distinct().ToArray(); }
        }

        // ---------- API اصلی ----------
        /// <summary>پیام فارسی کد پاسخ؛ «""» یعنی پیامی برای این ترکیب تعریف نشده (مثل کد قدیمی).</summary>
        public static bool TryGetMessage(string company, string responseCode, string actionCode, string rrn, out string message)
        {
            message = Resolve(company, responseCode, actionCode, rrn);
            return message != "";
        }

        /// <summary>پیام فارسی کد پاسخ؛ «""» یعنی پیامی برای این ترکیب تعریف نشده (مثل کد قدیمی).</summary>
        public static string GetMessage(string company, string responseCode, string actionCode, string rrn)
        {
            return Resolve(company, responseCode, actionCode, rrn);
        }

        /// <summary>همان GetMessage ولی با برند کارت‌خوان؛ نام نمایشی برند به کلید جدول ترجمه می‌شود.</summary>
        public static string GetMessage(PosBrand brand, string responseCode, string actionCode, string rrn)
        {
            return GetMessage(ResolveCompany(PosBrandInfo.Get(brand).DisplayName), responseCode, actionCode, rrn);
        }

        private static string Resolve(string company, string responseCode, string actionCode, string rrn)
        {
            // قواعد شرط‌دار (فن آوا / پرداخت نوین آرین): ترتیب مثل کد قدیمی — آخرین تطابق برنده است،
            // پس پیام‌های زنجیره else-if (که در انتها اضافه شده‌اند) قواعد قبلی را بازنویسی می‌کنند.
            List<PosResponseRule> rules;
            if (Rules.TryGetValue(company, out rules))
            {
                string last = null;
                string fallback = null;
                foreach (var r in rules)
                {
                    bool isFallback = r.Code == "*" || r.Code == "";
                    if (!isFallback && r.Code != responseCode)
                        continue;
                    if (r.ActionCode != null && r.ActionCode != "" && r.ActionCode != actionCode)
                        continue;
                    if (r.ExtraCondition != null && !r.ExtraCondition(rrn))
                        continue;
                    if (isFallback)
                        fallback = r.Message; // elseِ نهایی زنجیره قدیمی
                    else
                        last = r.Message; // آخرین تطابق برنده است (مثل کد قدیمی)
                }
                // بازگشت پیام قاعده؛ اگر هیچ قاعده‌ای تطابق نکرد، کد ثبت‌شده ساده (رجیستری) و بعد fallback
                if (last != null) return last;
                Dictionary<string, string> t;
                if (Messages.TryGetValue(company, out t) && t.TryGetValue(responseCode, out var msg)) return msg;
                return fallback ?? "";
            }

            Dictionary<string, string> table;
            if (Messages.TryGetValue(company, out table))
            {
                string message;
                if (table.TryGetValue(responseCode, out message))
                    return message;
            }
            return "";
        }

        /// <summary>تطبیق نام نمایشی رجیستری برند با کلیدهای قدیمی جدول (سداد/فناوا/سایان‌کارت/نوین آریان)</summary>
        private static string ResolveCompany(string displayName)
        {
            switch (displayName)
            {
                case "سداد": return "داده ورزی سداد";
                case "فناوا": return "فن آوا";
                case "سایان‌کارت": return "سایان کارت";
                case "نوین آریان": return "پرداخت نوین آرین";
                default: return displayName;
            }
        }

        // ---------- تونل توسعه (Open/Closed) ----------
        /// <summary>
        /// ثبت کد ساده برای بانک موجود یا جدید (کلید نام بانک آزاد است). اگر برای آن بانک قواعد شرط‌دار باشد،
        /// پیام ساده تا زمانی که هیچ قاعده‌ای تطابق نیابد به‌عنوان پیش‌فرض نهایی استفاده می‌شود.
        /// </summary>
        public static void Register(string company, string code, string message)
        {
            Dictionary<string, string> table;
            if (!Messages.TryGetValue(company, out table))
            {
                table = new Dictionary<string, string>(StringComparer.Ordinal);
                Messages[company] = table;
            }
            table[code] = message;
        }

        /// <summary>ثبت قاعده شرط‌دار (ActionCode/شرط اضافی) برای بانک موجود یا جدید؛ ترتیب ثبت، ترتیب ارزیابی است.</summary>
        public static void Register(string company, PosResponseRule rule)
        {
            RulesOf(company).Add(rule);
        }

        /// <summary>کد پاسخ را از جدول حذف می‌کند (برای بازنویسی پیش‌فرض‌ها از بالا)</summary>
        public static bool Unregister(string company, string code)
        {
            Dictionary<string, string> table;
            return Messages.TryGetValue(company, out table) && table.Remove(code);
        }
    }

    /// <summary>همان امضای کلاس قدیمی POS_Message؛ برای خوانا ماندن فراخوانی‌های قدیمی، فقط به جدول delegation می‌کند.</summary>
    public static class PosMessage
    {
        public static string AlertPost(string company, string result, string actionCode, string CompRRN)
        {
            return PosResponseMessageTable.GetMessage(company, result, actionCode, CompRRN);
        }
    }
}
