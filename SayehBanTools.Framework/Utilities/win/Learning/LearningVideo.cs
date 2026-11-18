using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Learning
{
    /// <summary>
    /// پخش فیلم آموزشی هر فرم.
    /// فیلم باید در پوشه Learning، با همان مسیرِ فرم در پوشه Forms، و با نام فرم بدون پیشوند Frm باشد.
    /// مثال: Forms\BaseInformation\ProductsAndGoods\MeasurementUnitSettings\FrmUnits
    ///   →  Learning\BaseInformation\ProductsAndGoods\MeasurementUnitSettings\Units.mp4  (هر پسوند ویدیویی)
    /// استفاده در فرم: <c>LearningVideo.Show(this);</c>
    /// قبل از استفاده، <see cref="FormsNamespace"/> را با namespace فرم‌های برنامه خود تنظیم کنید.
    /// </summary>
    public static class LearningVideo
    {
        /// <summary>پیشوند namespace فرم‌ها (مثال: «MyApp.Forms»). پیش‌فرض پوشه خالی است.</summary>
        public static string FormsNamespace { get; set; } = string.Empty;

        /// <summary>نام پوشه/محصول برای ذخیره محل توقف ویدیوها در LocalApplicationData</summary>
        public static string ProductName { get; set; } = "SayehBanTools";

        /// <summary>پسوندهایی که به‌عنوان فیلم شناخته می‌شوند (موتور VLC همه را پخش می‌کند)</summary>
        public static readonly string[] VideoExtensions =
        {
            ".mp4", ".mkv", ".avi", ".wmv", ".mov", ".m4v", ".webm", ".flv", ".f4v", ".mpg", ".mpeg",
            ".3gp", ".ts", ".mts", ".m2ts", ".vob", ".ogv", ".asf", ".divx", ".rm", ".rmvb"
        };

        /// <summary>پوشه Learning کنار برنامه</summary>
        public static string RootFolder
        {
            get { return Path.Combine(Application.StartupPath, "Learning"); }
        }

        /// <summary>فیلم آموزشی فرم را باز می‌کند؛ اگر فیلم نباشد پیام می‌دهد.</summary>
        /// <param name="form">فرمی که منوی آموزشی آن زده شده (معمولاً this)</param>
        /// <param name="videoName">اگر نام فیلم با نام فرم فرق دارد (بدون پسوند)</param>
        public static void Show(Form form, string videoName = null)
        {
            if (form == null) return;

            string folder = Path.Combine(RootFolder, GetRelativeFolder(form.GetType()));
            string name = string.IsNullOrWhiteSpace(videoName) ? GetVideoName(form.GetType()) : videoName.Trim();
            string file = FindVideo(folder, name);

            if (file == null)
            {
                MessageBox.Show(
                    "فیلم آموزشی این بخش هنوز اضافه نشده است.\n\nمسیر مورد انتظار:\n" + Path.Combine(folder, name + ".mp4"),
                    "آموزشی", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
                return;
            }

            string title = form.Text;
            FrmLearningPlayer.Open(file, string.IsNullOrWhiteSpace(title) ? name : title);
        }

        /// <summary>MyApp.Forms.A.B → A\B</summary>
        public static string GetRelativeFolder(Type formType)
        {
            string ns = formType.Namespace ?? string.Empty;
            if (!string.IsNullOrEmpty(FormsNamespace) && ns.StartsWith(FormsNamespace, StringComparison.Ordinal))
                ns = ns.Substring(FormsNamespace.Length);
            return ns.Trim('.').Replace('.', Path.DirectorySeparatorChar);
        }

        /// <summary>FrmUnits → Units ، Frm_Login → Login</summary>
        public static string GetVideoName(Type formType)
        {
            string name = formType.Name;
            if (name.StartsWith("Frm", StringComparison.OrdinalIgnoreCase) && name.Length > 3)
                name = name.Substring(3).TrimStart('_');
            return name;
        }

        /// <summary>فایل ویدیویی با این نام (هر پسوند)؛ اگر چند فایل بود، جدیدترین</summary>
        public static string FindVideo(string folder, string name)
        {
            if (!Directory.Exists(folder)) return null;
            return Directory.GetFiles(folder, name + ".*")
                .Where(f => string.Equals(Path.GetFileNameWithoutExtension(f), name, StringComparison.OrdinalIgnoreCase)
                            && VideoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// ذخیره محل توقف هر فیلم و تنظیمات پخش‌کننده (صدا، سرعت) برای کاربر ویندوز:
    /// %LocalAppData%\{ProductName}\LearningProgress.json
    /// اگر فیلم عوض شود (حجم یا تاریخ فایل)، از اول پخش می‌شود.
    /// </summary>
    internal static class LearningProgress
    {
        internal class Entry
        {
            public long PositionMs { get; set; }
            public long LengthMs { get; set; }
            public long FileSize { get; set; }
            public DateTime FileWriteUtc { get; set; }
            public DateTime LastWatchedUtc { get; set; }
        }

        internal class Data
        {
            public int Volume { get; set; } = 100;
            public float Rate { get; set; } = 1f;
            public bool TopMost { get; set; }
            public Dictionary<string, Entry> Videos { get; set; } = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        }

        private static readonly object Sync = new object();
        private static Data _data;

        private static string FilePath
        {
            get { return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LearningVideo.ProductName, "LearningProgress.json"); }
        }

        internal static Data Settings
        {
            get
            {
                lock (Sync)
                {
                    if (_data != null) return _data;
                    try
                    {
                        if (File.Exists(FilePath))
                            _data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(FilePath));
                    }
                    catch { /* فایل خراب: از نو */ }
                    if (_data == null) _data = new Data();
                    if (_data.Videos == null) _data.Videos = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
                    else _data.Videos = new Dictionary<string, Entry>(_data.Videos, StringComparer.OrdinalIgnoreCase);
                    return _data;
                }
            }
        }

        /// <summary>کلید هر فیلم: مسیر نسبی داخل پوشه Learning (بدون پسوند تا تغییر فرمت هم همان فیلم حساب شود)</summary>
        private static string KeyOf(string file)
        {
            string full = Path.GetFullPath(file);
            string root = Path.GetFullPath(LearningVideo.RootFolder).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string rel = full.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? full.Substring(root.Length) : full;
            return Path.Combine(Path.GetDirectoryName(rel) ?? string.Empty, Path.GetFileNameWithoutExtension(rel));
        }

        /// <summary>محل ادامه پخش (میلی‌ثانیه)؛ صفر یعنی از اول</summary>
        internal static long GetResumePosition(string file)
        {
            lock (Sync)
            {
                Entry e;
                if (!Settings.Videos.TryGetValue(KeyOf(file), out e)) return 0;
                var fi = new FileInfo(file);
                if (e.FileSize != fi.Length || e.FileWriteUtc != fi.LastWriteTimeUtc) return 0;   // فیلم به‌روز شده
                return e.PositionMs;
            }
        }

        internal static void SavePosition(string file, long positionMs, long lengthMs)
        {
            lock (Sync)
            {
                var fi = new FileInfo(file);
                // چند ثانیه اول یا آخر: یعنی شروع نکرده یا تا آخر دیده؛ دفعه بعد از اول
                if (positionMs < 5000 || (lengthMs > 0 && positionMs > lengthMs - 5000)) positionMs = 0;
                Settings.Videos[KeyOf(file)] = new Entry
                {
                    PositionMs = Math.Max(0, positionMs),
                    LengthMs = lengthMs,
                    FileSize = fi.Exists ? fi.Length : 0,
                    FileWriteUtc = fi.Exists ? fi.LastWriteTimeUtc : DateTime.MinValue,
                    LastWatchedUtc = DateTime.UtcNow
                };
                Save();
            }
        }

        internal static void Save()
        {
            lock (Sync)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                    string tmp = FilePath + ".tmp";
                    File.WriteAllText(tmp, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                    if (File.Exists(FilePath)) File.Replace(tmp, FilePath, null);
                    else File.Move(tmp, FilePath);
                }
                catch { /* ذخیره نشدن محل توقف نباید برنامه را متوقف کند */ }
            }
        }
    }
}
