# SayehBanTools.Framework

[![NuGet](https://img.shields.io/nuget/v/SayehBanTools.Framework.svg)](https://www.nuget.org/packages/SayehBanTools.Framework)
[![Downloads](https://img.shields.io/nuget/dt/SayehBanTools.Framework.svg)](https://www.nuget.org/packages/SayehBanTools.Framework)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)
![WinForms](https://img.shields.io/badge/UI-WinForms-0078D4)

مجموعه‌ای از ابزارها و کامپوننت‌های کاربردی برای **.NET Framework 4.8**: از کنترل‌های آماده WinForms (جدول، صفحه‌بندی، تکست‌باکس هوشمند، کمبوباکس با جستجو) تا تاریخ شمسی، تبدیل عدد به حروف، رمزنگاری، اعتبارسنجی کد ملی، پیامک، Redis، RabbitMQ، Elasticsearch و هوش مصنوعی محلی (Ollama).

> 💡 **دو نسخه پکیج:** این کتابخانه به‌صورت دوقلو منتشر می‌شود — [SayehBanTools](https://www.nuget.org/packages/SayehBanTools) برای .NET Modern (net11.0-windows) و **SayehBanTools.Framework** برای .NET Framework 4.8. کد هر دو نسخه یکسان است و هر دو به‌طور موازی به‌روزرسانی می‌شوند؛ نسخه‌ای را نصب کنید که با Target Framework پروژه شما سازگار است.

---

## کارت‌خوان PC-POS

اتصال به کارت‌خوان‌های PC-POS بانکی (پروتکل CPML) از طریق پورت سریال یا شبکه — namespace `SayehBanTools.Framework.PCPOS`:

```csharp
var settings = new PosConnectionSettings
{
    Brand = PosBrand.SayanKart,
    ConnectionType = PosConnectionType.SerialPort,
    SerialPort = "COM3",
    BaudRate = 38400
};

var pos = PosCommander.Instance;
if (pos.Init(settings))
{
    Response result = pos.Purchase("فاکتور ۱۲۳", "50000", print: true);
}
```

سایر عملیات: `BalanceInquiry`، `BillPayment`، `LastTransaction`، `PrintText`. کدهای خطای `Response.ErrorCode`: 201 عدم اتصال، 202 خطای ارسال، 203 پاسخ خالی، 204 پاسخ نامعتبر.

---

## پخش فیلم آموزشی

`LearningVideo` + `FrmLearningPlayer` (namespace `SayehBanTools.Framework.Utilities.win.Learning`) با موتور VLC محل توقف فیلم را برای ادامه‌ی دوباره ذخیره می‌کند:

```csharp
LearningVideo.Show(this);   // پوشه Learning کنار exe
```

پکیج به `LibVLCSharp` و `LibVLCSharp.WinForms` وابسته است؛ برای موتور بومی، بسته `VideoLAN.LibVLC.Windows` را به **برنامه مصرف‌کننده** اضافه کنید:

```xml
<PackageReference Include="VideoLAN.LibVLC.Windows" Version="3.0.21" />
```

---

## فهرست

- [نصب](#نصب)
- [کارت‌خوان PC-POS](#کارتخوان-pc-pos)
- [پخش فیلم آموزشی](#پخش-فیلم-آموزشی)
- [کامپوننت‌های WinForms](#کامپوننتهای-winforms)
- [ابزارهای کمکی WinForms](#ابزارهای-کمکی-winforms)
- [تاریخ شمسی](#تاریخ-شمسی)
- [رشته و عدد](#رشته-و-عدد)
- [محاسبات](#محاسبات)
- [تولید و اعتبارسنجی کد](#تولید-و-اعتبارسنجی-کد)
- [رمزنگاری و رشته اتصال](#رمزنگاری-و-رشته-اتصال)
- [داده و صفحه‌بندی](#داده-و-صفحهبندی)
- [شبکه و IP](#شبکه-و-ip)
- [پیامک](#پیامک)
- [هوش مصنوعی محلی (Ollama)](#هوش-مصنوعی-محلی-ollama)
- [سرویس‌های زیرساختی](#سرویسهای-زیرساختی)
- [Attributeهای اعتبارسنجی](#attributeهای-اعتبارسنجی)
- [وابستگی‌ها](#وابستگیها)
- [ارتباط](#ارتباط)

---

## نصب

```bash
dotnet add package SayehBanTools.Framework
```

یا از Package Manager Console:

```powershell
Install-Package SayehBanTools.Framework
```

> پیش‌نیاز: پروژه با هدف **.NET Framework 4.8**.

---

## کامپوننت‌های WinForms

همه کنترل‌ها در فضای نام `SayehBanTools.Framework.Utilities.win.component.CommonControls` هستند. بعد از نصب پکیج و یک بار Build، در Toolbox ویژوال استودیو نمایش داده می‌شوند.

| کنترل | کاربرد |
|---|---|
| `DGV` | DataGridView با شماره ردیف، رنگ یک‌درمیان، ذخیره و بازیابی ترتیب ستون‌ها و خروجی اکسل با دوبار کلیک روی هدر |
| `DGVPagin` | همان `DGV` با نوار صفحه‌بندی داخلی و ستون انتخاب همه |
| `SmartTextBox` | تکست‌باکس با حالت‌های متن، فقط حروف، عدد، مبلغ (جداکننده هزارگان) و تاریخ، به همراه Watermark |
| `cmbbox` | کمبوباکس با جستجو، رنگ فوکوس و رفتن به کنترل بعدی با Enter |
| `cmbboxDGV` | کمبوباکس با جستجوی ریموت (async) برای انتخاب از داده‌های زیاد |
| `Btn` | دکمه گرد با افکت Glow و نماد ویژه |
| `PanelGlass` | پنل شیشه‌ای با عنوان |
| `PicBox` / `PicBoxShow` | پیکچرباکس با بارگذاری و دریافت تصویر به صورت `byte[]` |

### DGV

```csharp
dgvItems.AlteraningColor = Color.Ivory;  // رنگ ردیف‌های یک‌درمیان
dgvItems.GONextCell = true;              // با Enter به سلول بعدی برود
dgvItems.DataSource = list;

dgvItems.RestoreColumnOrders();          // در Load فرم
dgvItems.SaveColumnOrders();             // در FormClosing
```

### DGVPagin

`DGVPagin` خودش Dock = Fill می‌شود. برای جای‌گذاری آن را داخل یک Panel جداگانه قرار دهید.

```csharp
dgvList.PageSize = 50;
dgvList.PageChanged += async (s, e) => await LoadPageAsync(e.PageNumber, e.PageSize);

private async Task LoadPageAsync(int page, int size)
{
    var result = await _service.GetPagedAsync(page, size);
    dgvList.DataSource = result.Items;
    dgvList.SetPagingInfo(result.TotalCount, page);
}

// اختیاری: ستون چک‌باکس با انتخاب همه در هدر
dgvList.EnableSelectAllCheckbox("colSelect");
```

### SmartTextBox

```csharp
txtPrice.InputType = SmartTextBox.TargetInputType.Money;
txtPrice.Watermark = "مبلغ (ریال)";

decimal price = decimal.Parse(txtPrice.UnformattedValue); // مقدار بدون جداکننده

txtDate.InputType = SmartTextBox.TargetInputType.Date;
txtDate.IsGregorianDate = false; // تاریخ شمسی
```

حالت‌های `InputType`: `Normal`، `CharacterOnly`، `Numeric`، `Money`، `Date`.

### cmbboxDGV (جستجوی ریموت)

```csharp
cmbProduct.RemoteSearchRequested += async text =>
    (await _products.SearchAsync(text)).Cast<object>();

if (cmbProduct.HasValidSelection())
{
    var product = (ProductDto)cmbProduct.SafeSelectedItem;
}
```

### PicBox

```csharp
picLogo.SetImageFromBytes(bytesFromDb);
byte[] bytes = picLogo.GetImageBytes();
picLogo.ClearImage();
```

### PanelGlass

```csharp
panelHeader.Title = "اطلاعات فاکتور";
panelHeader.TitleColor = Color.Navy;
panelHeader.TitleAlignment = HorizontalAlignment.Right;
```

---

## ابزارهای کمکی WinForms

فضای نام: `SayehBanTools.Framework.Utilities.win.Helper`

```csharp
// آیکن و عنوان فرم
this.SetAppIcon(iconBytes);
this.SetAppTitle("فروشگاه مرکزی");

// خروج با تأیید کاربر
if (SystemHelper.ConfirmExit()) SystemHelper.ExitApp();

// اعلان ویندوز
WindowsNotifier.Show("ذخیره", "فاکتور با موفقیت ثبت شد");

// خروجی اکسل از DataTable (نیازمند Microsoft Excel روی سیستم)
new ExportaExcel().Exporta(dataTable);
```

فرمت خودکار ستون‌های جدول در رویداد `CellFormatting`:

```csharp
var config = new Dictionary<string, GridFormatConfig>
{
    ["colCreatedAt"] = new GridFormatConfig(1),                     // تاریخ و ساعت شمسی از UTC
    ["colInvoiceDate"] = new GridFormatConfig(2),                   // فقط تاریخ شمسی از UTC
    ["colIsActive"] = new GridFormatConfig(3, "فعال", "غیرفعال"),  // نمایش bool به صورت متن
};

private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    => DGV_Helper.ApplyGridFormatting(dgv, e, config);
```

---

## تاریخ شمسی

فضای نام: `SayehBanTools.Framework.ShowDateTime`

```csharp
string d1 = ConvertDateTime.MiladiToShamsi(DateTime.Now);              // 1405/08/16
DateTime d2 = ConvertDateTime.ShamsiToMiladi("1405/08/16");

// برای مقادیری که به صورت UTC ذخیره شده‌اند (به وقت ایران نمایش می‌دهد)
string d3 = ConvertDateTime.ConvertToPersianDateUTC(invoice.InvoiceDateUtc);
string d4 = ConvertDateTime.ConvertToPersianDateTimeUTC(invoice.InvoiceDateUtc);

string d5 = ConvertDateTime.miladi2shamsi(DateTime.Now);               // متن کامل با نام روز و ماه
string year = ConvertDateTime.GetCurrentYearPersion();
```

برای Dapper و JSON هم `ManageDateAndTime.DateOnlyTypeHandler` و `ManageDateAndTime.DateOnlyJsonConverter` (فضای نام `Converter`) آماده است.

---

## رشته و عدد

```csharp
using SayehBanTools.Framework.Converter;

"123".En2Fa();                 // ۱۲۳
"۱۲۳".Fa2En();                 // 123
"كيك".FixPersianChars();       // کیک (ی و ک عربی به فارسی)
"<b>سلام</b>".StripHtml();     // سلام
"Hello World".Slug();          // Hello-World
"   ".HasValue();              // false
"".NullIfEmpty();              // null
1250000m.ToCurrency("fa");     // جداکننده هزارگان

// پشتیبانی از ۹۹ زبان (کد دوحرفی یا منطقه‌ای: "hi" یا "hi-IN")
"123".NormalizeByLanguage("hi");          // १२३ (رقم بومی)
"١٢٣".NormalizeByLanguage("fa");          // ۱۲۳ (عربی ← فارسی)
"१२३".ToInt("hi");                        // 123
"۱۲٫۵".ToDecimal("fa");                   // 12.5
"៣៤".ToEnglishDigits();                   // 34 (ارقام هر خطی)
"123".ToNativeDigits("th");               // ๑๒๓
1234567.ToNumeric("de");                  // 1.234.567
"ur".IsRtlLanguage();                     // true
"fa-IR".GetTextDirection();               // rtl
SupportedLanguages.Find("ja").NativeName; // 日本語
SupportedLanguages.List;                  // فهرست ۹۹ زبان (Id، Code، RegionCode، NativeName، TextDirection، Culture)

// عدد به حروف
ConvertNumbertToString.ConvertRaghamToHorof(1250);   // یک هزار و دویست و پنجاه
ConvertNumToString.convert("1250");

// IP
long n = "192.168.1.10".IPToNumber();
string ip = n.NumberToIPAddress();
bool ok = "10.0.0.1".IsValidIPAddress();
```

---

## محاسبات

```csharp
using SayehBanTools.Framework.Calc;

Calculator.Add(5, 10, 15);          // 30
Calculator.Multiply(2, 3, 4);       // 24
Calculator.Discount(1000m, 20);     // 800
Calculator.Taxation(1000m, 10);     // 1100
Calculator.GetPercent(250m, 1000m); // 25
```

---

## تولید و اعتبارسنجی کد

```csharp
using SayehBanTools.Framework.GenerateValue;

// کد ملی ۱۰ رقمی (الگوریتم ثبت احوال؛ ارقام فارسی و خط‌تیره هم پذیرفته می‌شود)
NationalCode.IsValidNationalCode("0014401509");       // true
NationalCode.GenerateRandom();                        // تصادفی، محل صدور تصادفی
NationalCode.GenerateRandom("001");                   // تصادفی در محل صدور 001
NationalCode.GenerateRound();                         // رند، مثلاً 1005555559
NationalCode.GenerateRound("001");                    // رند در محل صدور 001، مثلاً 0015555550

// کد ملی ۲۰ رقمی: [کشور ۳][محل صدور ۳][سریال ۷][پسوند ۵][کنترل ۲]
NationalCode.ConvertTo20("0014401509");               // 36400144015090000082 (کد قبلی دقیقاً داخلش است)
NationalCode.ConvertTo10("36400144015090000082");     // 0014401509
NationalCode.GenerateNear20("0014401509");            // کد ۲۰ رقمی جدید در همان محل صدور
NationalCode.GenerateRandom20();                      // تصادفی (ایران 364)
NationalCode.GenerateRound20("364", "001");           // رند در محل صدور 001
NationalCode.IsValidNationalCode20("36400144015090000082"); // true

NationalCode.IsValid("0014401509");                   // ۱۰ یا ۲۰ رقمی، بدون استثنا
NationalCode.Format("36400144015090000082");          // 364-001-4401509-00000-82
NationalCode.GetLocationCode("0014401509");           // 001

LegalNationalID.IsValidLegalNationalID("10101234567"); // شناسه ملی اشخاص حقوقی
string code = GenerateValues.Generate16ValueRandome(); // رشته تصادفی ۱۶ رقمی
```

ساختار کد ۲۰ رقمی:
- **کشور (۳ رقم):** کد عددی ISO 3166-1 (ایران = `364`) تا کشورهای دیگر در آینده بدون تداخل اضافه شوند.
- **محل صدور (۳ رقم):** همان سه رقم اول کد ۱۰ رقمی.
- **سریال (۷) + پسوند (۵):** در کد تبدیل‌شده از ۱۰ رقمی، محل صدور + سریال دقیقاً همان کد قبلی است و پسوند `00000`؛ کدهای جدید همیشه پسوند غیرصفر دارند، پس هیچ‌وقت با کدهای تبدیل‌شده تداخل ندارند (ظرفیت هر محل صدور حدود ۱۰^۱۲).
- **کنترل (۲ رقم):** هر دو با همان قاعده ثبت احوال (`r = مجموع % 11`؛ `r < 2 ? r : 11 - r`)؛ رقم اول با وزن‌های ۱۰..۲ تکرارشونده روی ۱۸ رقم، رقم دوم با وزن‌های ۱..۱۰ روی ۱۹ رقم. هر خطای تک‌رقمی و هر جابه‌جایی دو رقم شناسایی می‌شود.

---

## رمزنگاری و رشته اتصال

```csharp
using SayehBanTools.Framework.Encryptor;
using SayehBanTools.Framework.ConnectionDB;

string enc = await StringEncryptor.EncryptAsync("secret", initVector, passPhrase);
string dec = await StringEncryptor.DecryptAsync(enc, initVector, passPhrase);

// رشته اتصال SQL Server (ساده یا با مقادیر رمزشده)
string cs = SqlServerConnection.ConnectionString(".", "MyDb", "sa", "pass");
string csEnc = await SqlServerConnection.ConnectionString(encServer, encDb, encUser, encPass, initVector, passPhrase);
```

---

## داده و صفحه‌بندی

```csharp
using SayehBanTools.Framework.Extensions;
using SayehBanTools.Framework.Utilities.Mapper;

// صفحه‌بندی روی IQueryable / IEnumerable
var page = products.ToPaged(page: 2, pageSize: 20, out int total);

// تبدیل لیست به DataTable (مثلاً برای Table-Valued Parameter در SQL Server)
DataTable tvp = ManageDataTable.ConvertToDataTable(items, "dbo.InvoiceDetailType");
```

---

## شبکه و IP

```csharp
using SayehBanTools.Framework.NetworkUtilities;
using SayehBanTools.Framework.AccessDenied;

bool online = new NetworkChecker().CheckInternetConnection();
string localIp = NetworkChecker.GetLocalIPAddress();
string publicIp = NetworkChecker.GetPublicIPAddress();

bool allowed = IPAccess.IsInRange("192.168.1.5", "192.168.1.1", "192.168.1.10");
```

---

## پیامک

```csharp
using SayehBanTools.Framework.Sender;

var data = new Dictionary<string, object> { ["code"] = "12345" };
var (response, content) = await SMS_System.SendPatternAsync(
    apiLink, apiKey, data, "pattern_code", "sender_number", "09120000000", null);

var credit = await SMS_System.GetCreditAsync(apiLink, apiKey);
```

---

## هوش مصنوعی محلی (Ollama)

```csharp
using SayehBanTools.Framework.AI.Ollama.llama;
using SayehBanTools.Framework.AI.Ollama.gemma;

using (var translator = new LocalAiTranslator("http://localhost:11434", "llama3"))
{
    await translator.InitializeAsync();
    string fa = await translator.TranslateAsync("Hello world", "en", "fa");
}

var vision = new LocalAiVisionAssistant("http://localhost:11434", "gemma3");
await vision.InitializeAsync();
string description = await vision.DescribeImageAsync(@"C:\images\product.jpg");
```

---

## سرویس‌های زیرساختی

| بخش | کلاس‌ها |
|---|---|
| Redis | `ICache`، `RCache`، `CacheManager` |
| RabbitMQ | `ISendMessages`، `RabbitMQMessageBus`، `RabbitMqConnectionSettings`، `ConfigureServicesRabbitMQ` |
| Elasticsearch | `IElasticsearch`، `RElasticsearch`، `ElasticsearchIndexManager` |
| ترجمه گوگل | `TranslateTexts.TranslateTextAsync(TranslationRequest)` |
| فایل | `ManageFiles` (آپلود معمولی و تکه‌ای، حذف، تبدیل تصویر به `byte[]` با تغییر اندازه) |

نمونه RabbitMQ:

```csharp
services.AddTransient<RabbitMQConnection>();
services.AddTransient<ISendMessages, RabbitMQMessageBus>();

await _messageBus.SendMessageAsync(message, "exchange_name", "queue_name");
```

---

## Attributeهای اعتبارسنجی

فضای نام `SayehBanTools.Framework.Validations.Attributes` شامل Attributeهای آماده برای مدل‌هاست:

`CommonPassword` · `Guid` · `HierarchyPath` · `IpAddressV4` · `IpAddressV6` · `PositiveNumberId` · `PositiveNumberIdArray` · `RowVersion`

```csharp
public class UserModel
{
    [CommonPassword("App_Data/worst-passwords.txt")]  // رد رمزهای عبور رایج و ضعیف
    public string Password { get; set; }

    [IpAddressV4]
    public string LastIp { get; set; }
}
```

---

## وابستگی‌ها

Dapper · Newtonsoft.Json · Microsoft.Extensions.Options · Microsoft.Office.Interop.Excel · StackExchange.Redis · RabbitMQ.Client · Elastic.Clients.Elasticsearch · OllamaSharp · RestSharp · System.IO.Abstractions · System.Reactive.Windows.Forms

---

## ارتباط

- 🌐 وب‌سایت: [sayehban.ir](https://sayehban.ir)
- 📧 ایمیل: sdvp1991david@gmail.com
- 💻 گیت‌هاب: [SayyehBan/SayehBanTools.Framework](https://github.com/SayyehBan/SayehBanTools.Framework)
- 📦 NuGet: [SayehBanTools.Framework](https://www.nuget.org/packages/SayehBanTools.Framework)

پیشنهاد، گزارش باگ و Pull Request در گیت‌هاب با کمال میل پذیرفته می‌شود.
