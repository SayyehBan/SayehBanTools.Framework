# SayehBanTools - راهنمای استفاده

![SayehBanTools Logo](https://github.com/SayyehBan/SayyehBanTools/logo.png)

یک کتابخانه چندمنظوره برای توسعه‌دهندگان دات‌نت با قابلیت‌های متنوع

## فهرست مطالب

- [نصب](#نصب)
- [کلاس‌ها و قابلیت‌ها](#کلاسها-و-قابلیتها)
  - [IPAccess](#ipaccess)
  - [Calculator](#calculator)
  - [RabbitMQ](#rabbitmq)
  - [مدیریت فایل‌ها](#مدیریت-فایلها)
  - [تبدیل اعداد](#تبدیل-اعداد)
  - [مدیریت رشته‌ها](#مدیریت-رشتهها)
  - [رمزنگاری](#رمزنگاری)
  - [تولید مقادیر](#تولید-مقادیر)
  - [ارسال پیامک](#ارسال-پیامک)
  - [مدیریت تاریخ](#مدیریت-تاریخ)
  - [ترجمه](#ترجمه)
  - [صفحه‌بندی](#صفحهبندی)
- [مثال‌های کد](#مثالهای-کد)
- [مشارکت](#مشارکت)
- [مجوز](#مجوز)

## نصب

برای نصب کتابخانه از NuGet استفاده کنید:

```bash
dotnet add package SayehBanTools
```

یا در Visual Studio از طریق Package Manager Console:

```powershell
Install-Package SayehBanTools
```

## کلاس‌ها و قابلیت‌ها

### IPAccess

بررسی محدوده IP:

```csharp
bool isInRange = IPAccess.IsInRange("192.168.1.5", "192.168.1.1", "192.168.1.10");
// true اگر IP در محدوده باشد
```

### Calculator

انجام محاسبات ریاضی:

```csharp
// جمع
double sum = Calculator.Add(5, 10, 15); // 30

// ضرب
double product = Calculator.Multiply(2, 3, 4); // 24

// تخفیف
decimal discounted = Calculator.Discount(1000, 20); // 800

// مالیات
decimal taxed = Calculator.Taxation(1000, 9); // 1090
```

### RabbitMQ

#### پیکربندی سرویس‌ها:

```csharp
services.AddTransient<RabbitMQConnection, RabbitMQConnection>();
services.AddTransient<ISendMessages, RabbitMQMessageBus>();
```

#### ارسال پیام:

```csharp
var message = new BaseMessage();
await _messageBus.SendMessageAsync(message, "exchange_name", "queue_name");
```

### مدیریت فایل‌ها

```csharp
var fileManager = new ManageFiles();

// آپلود فایل
string uploadedPath = await fileManager.UploadFileAsync("/uploads", file);

// حذف فایل
fileManager.DeleteFileServer("/uploads/file.txt");
```

### تبدیل اعداد

```csharp
// عدد به حروف
string words = ConvertNumToString.convert("123456789");
// "صد و بیست و سه میلیون و چهارصد و پنجاه و شش هزار و هفتصد و هشتاد و نه"

// عدد به رشته فارسی
string persianNumber = "123456".En2Fa(); // "۱۲۳۴۵۶"
```

### مدیریت رشته‌ها

```csharp
// تبدیل اعداد انگلیسی به فارسی
string persianNum = "123".En2Fa(); // "۱۲۳"

// تبدیل اعداد فارسی به انگلیسی
string englishNum = "۱۲۳".Fa2En(); // "123"

// حذف تگ‌های HTML
string cleanHtml = "<div>Test</div>".HtmlTags(); // "Test"
```

### رمزنگاری

```csharp
// رمزنگاری
string encrypted = await StringEncryptor.EncryptAsync("text", "initVector", "passPhrase");

// رمزگشایی
string decrypted = await StringEncryptor.DecryptAsync(encrypted, "initVector", "passPhrase");
```

### تولید مقادیر

```csharp
// کد ملی معتبر
string nationalCode = NationalCode.GenerateRandom();

// کد تصادفی 16 رقمی
string randomValue = GenerateValues.Generate16ValueRandome();
```

### ارسال پیامک

```csharp
// ارسال پیامک پترن
var data = new Dictionary<string, object> { { "code", "12345" } };
var result = await SMS_System.SendPatternAsync(apiLink, apiKey, data, "pattern_code", "sender", "09123456789", null);

// بررسی اعتبار
var credit = await SMS_System.GetCreditAsync(apiLink, apiKey);
```

### مدیریت تاریخ

```csharp
// تبدیل میلادی به شمسی
string shamsiDate = ConvertDateTime.MiladiToShamsi(DateTime.Now);

// تبدیل شمسی به میلادی
DateTime miladiDate = ConvertDateTime.ShamsiToMiladi("1402/05/15");
```

### ترجمه

```csharp
var translator = new TranslateTexts(new HttpClient());
var request = new TranslationRequest
{
    OriginalText = "Hello",
    InputLanguage = "en",
    OutputLanguage = "fa"
};

var response = await translator.TranslateTextAsync(request);
// response.Translations[0] = "سلام"
```

### صفحه‌بندی

```csharp
var query = dbContext.Products.AsQueryable();
var pager = new Pager(totalItems: 100, currentPage: 2, pageSize: 10);

var pagedResults = query.PagedResult(pager.CurrentPage, pager.PageSize, out int totalCount);
```

## مثال‌های کد

### مثال کامل RabbitMQ

```csharp
// Startup.cs
services.Configure<RabbitMqConnectionSettings>(Configuration.GetSection("RabbitMQ"));
services.AddTransient<RabbitMQConnection>();
services.AddTransient<ISendMessages, RabbitMQMessageBus>();

// Controller
public class MessageController : Controller
{
    private readonly ISendMessages _messageBus;
    
    public MessageController(ISendMessages messageBus)
    {
        _messageBus = messageBus;
    }
    
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] MessageDto message)
    {
        var baseMessage = new BaseMessage();
        await _messageBus.SendMessageAsync(baseMessage, "notifications", "emails");
        return Ok();
    }
}
```

### مثال مدیریت فایل

```csharp
[HttpPost]
public async Task<IActionResult> Upload(IFormFile file)
{
    var fileManager = new ManageFiles();
    string path = await fileManager.UploadFileAsync("wwwroot/uploads", file);
    return Ok(new { Path = path });
}
```

## مشارکت

مشارکت‌های شما استقبال می‌شود! برای مشارکت:

1. ریپوی را Fork کنید
2. شاخه جدید ایجاد کنید (`git checkout -b feature/AmazingFeature`)
3. تغییرات را Commit کنید (`git commit -m 'Add some AmazingFeature'`)
4. Push به شاخه (`git push origin feature/AmazingFeature`)
5. Pull Request باز کنید

## مجوز

این پروژه تحت مجوز MIT منتشر شده است. برای اطلاعات بیشتر فایل [LICENSE](LICENSE) را مطالعه کنید.

---

📧 برای ارتباط: sdvp1991david@gmail.com  
🌍 وبسایت: [https://sayehban.ir](https://sayehban.ir)  
💻 مخزن کد: [https://github.com/SayyehBan/SayehBanTools](https://github.com/SayyehBan/SayehBanTools)