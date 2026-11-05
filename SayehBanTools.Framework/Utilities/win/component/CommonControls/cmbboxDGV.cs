using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    /// <summary>
    /// کامبوباکس اختصاصی با قابلیت نمایش تصویر و متن به صورت یکپارچه (OwnerDraw)
    /// رفع مشکل DisplayMember و تبدیل هوشمند آرایه برای جلوگیری از چاپ نام کلاس
    /// </summary>
    public class cmbboxDGV : ComboBox
    {
        #region Win32 API برای جلوگیری از خطای خواندن متن

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        public string RawText
        {
            get
            {
                if (!IsHandleCreated) return string.Empty;
                int len = GetWindowTextLength(Handle);
                if (len <= 0) return string.Empty;
                StringBuilder sb = new StringBuilder(len + 1);
                GetWindowText(Handle, sb, sb.Capacity);
                return sb.ToString();
            }
        }

        /// <summary>
        /// باگ WinForms: بعد از عوض شدن DataSource (مثلاً نتیجه جستجوی خالی) ممکن است ایندکس انتخاب در کنترل
        /// ویندوز (native) هنوز 0 باشد در حالی که لیست آیتم‌ها خالی است؛ آن‌وقت خواندن Text در پیام‌های ماوس
        /// خطای ArgumentOutOfRangeException می‌دهد و برنامه کرش می‌کند. در این حالت متن خام را برمی‌گردانیم.
        /// </summary>
        public override string Text
        {
            get
            {
                try { return base.Text; }
                catch (ArgumentOutOfRangeException)
                {
                    ResetNativeSelection();
                    return RawText;
                }
            }
            set { base.Text = value; }
        }

        /// <summary>
        /// آیتم انتخاب شده به صورت امن (در صورت ناهماهنگی ایندکس، null برمی‌گرداند)
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public object SafeSelectedItem
        {
            get
            {
                try
                {
                    int index = SelectedIndex;
                    return (index >= 0 && index < Items.Count) ? Items[index] : null;
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// انتخاب معتبر است فقط اگر: آیتمی انتخاب شده باشد، مقدار داشته باشد و
        /// متن تایپ‌شده دقیقاً با متن همان آیتم یکی باشد.
        /// (بعد از جستجو، کنترل خودکار اولین نتیجه را انتخاب می‌کند؛ پس متن ناقص یا اشتباه
        /// بدون این بررسی، اولین کالای لیست را به اشتباه ذخیره می‌کرد)
        /// </summary>
        public bool HasValidSelection()
        {
            try
            {
                object item = SafeSelectedItem;
                if (item == null || SelectedValue == null) return false;

                string typed = (RawText ?? string.Empty).Trim();
                string itemText = (GetItemText(item) ?? string.Empty).Trim();
                return typed.Length > 0 && string.Equals(typed, itemText, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        private const int CB_SETCURSEL = 0x014E;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void ResetNativeSelection()
        {
            try
            {
                if (IsHandleCreated) SendMessage(Handle, CB_SETCURSEL, (IntPtr)(-1), IntPtr.Zero);
            }
            catch { }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                // همان ناهماهنگی ایندکس native/managed در پیام‌های دیگر (مثلاً SelectedItem)؛ کرش نکند
                ResetNativeSelection();
            }
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            // اگر فرم از بیرون DataSource را عوض کرد (ریست فرم، انتخاب از گرید)،
            // کلمه جستجوی قبلی فراموش شود تا تایپ دوباره همان متن، جستجو را اجرا کند
            if (!_isUpdatingText) _lastKeyword = string.Empty;
            base.OnDataSourceChanged(e);
        }

        #endregion

        private Timer _searchTimer;
        private string _lastKeyword = string.Empty;
        private bool _isUpdatingText = false;

        private Dictionary<object, Image> _imageCache = new Dictionary<object, Image>();

        public event Func<string, Task<IEnumerable<object>>> RemoteSearchRequested;

        public cmbboxDGV()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 55;
            this.DropDownStyle = ComboBoxStyle.DropDown;
            this.RightToLeft = RightToLeft.Yes;
            this.Cursor = Cursors.Hand;

            this.AutoCompleteMode = AutoCompleteMode.None;
            this.AutoCompleteSource = AutoCompleteSource.None;

            _searchTimer = new Timer { Interval = 400 };
            _searchTimer.Tick += SearchTimer_Tick;
        }

        #region رسم سفارشی لیست (OwnerDraw)

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;

            e.DrawBackground();

            object item = Items[e.Index];

            // ۱. استخراج متن (اگر ویندوز فرم به اشتباه نام کلاس را داد، خودمان مستقیماً از پراپرتی می‌خوانیم)
            string text = GetItemText(item);
            if (text == item.GetType().FullName)
            {
                var prop = item.GetType().GetProperty(this.DisplayMember ?? "ProductName");
                if (prop != null)
                {
                    text = prop.GetValue(item)?.ToString() ?? text;
                }
            }

            // ۲. استخراج و رسم تصویر
            Image img = GetItemImage(item);
            Rectangle imgRect = new Rectangle(e.Bounds.Right - 55, e.Bounds.Top + 5, 45, 45);

            if (img != null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawImage(img, imgRect);
            }

            Color textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? Color.White
                : ForeColor;

            TextFormatFlags flags = TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;

            // ۳. رسم عنوان کالا
            Rectangle textRect = new Rectangle(e.Bounds.Left + 5, e.Bounds.Top, e.Bounds.Width - 65, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, text, new Font("B Koodak", 11F, FontStyle.Bold), textRect, textColor, flags);

            // ۴. رسم خط جداکننده زیر هر آیتم
            using (var pen = new Pen(Color.FromArgb(230, 230, 230)))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            e.DrawFocusRectangle();
        }

        private Image GetItemImage(object item)
        {
            if (_imageCache.TryGetValue(item, out Image cachedImg))
                return cachedImg;

            var prop = item.GetType().GetProperty("ProductImage");
            if (prop != null)
            {
                if (prop.GetValue(item) is byte[] imgBytes && imgBytes.Length > 0)
                {
                    try
                    {
                        using (var ms = new MemoryStream(imgBytes))
                        {
                            Image img = Image.FromStream(ms);
                            _imageCache[item] = img;
                            return img;
                        }
                    }
                    catch { }
                }
            }

            _imageCache[item] = null;
            return null;
        }

        private void ClearImageCache()
        {
            foreach (var img in _imageCache.Values)
            {
                img?.Dispose();
            }
            _imageCache.Clear();
        }

        #endregion

        #region تایپ هوشمند و واکشی از دیتابیس

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);

            if (_isUpdatingText) return;

            string currentTypedText = this.RawText;

            if (string.IsNullOrWhiteSpace(currentTypedText))
            {
                _searchTimer.Stop();
                this.DroppedDown = false;
                return;
            }

            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private async void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            string keyword = this.RawText.Trim();

            if (keyword == _lastKeyword || string.IsNullOrWhiteSpace(keyword)) return;
            _lastKeyword = keyword;

            if (RemoteSearchRequested != null)
            {
                var results = await RemoteSearchRequested(keyword);
                if (results != null)
                {
                    _isUpdatingText = true;

                    string currentText = this.RawText;
                    int currentSelectionStart = 0;
                    try { currentSelectionStart = this.SelectionStart; } catch { }

                    string savedDisplayMember = this.DisplayMember;
                    string savedValueMember = this.ValueMember;

                    ClearImageCache();

                    // بستن لیست باز و پاک کردن ایندکس native قبل از تعویض دیتاسورس (جلوگیری از ایندکس قدیمی)
                    if (this.DroppedDown) this.DroppedDown = false;
                    ResetNativeSelection();

                    // با نال کردن دیتاسورس، DisplayMember در پس‌زمینه پاک می‌شود
                    this.DataSource = null;

                    // --- تبدیل لیست object به آرایه داینامیک از نوع واقعی کلاس ---
                    var resultsList = new List<object>(results);
                    if (resultsList.Count > 0)
                    {
                        Type actualType = resultsList[0].GetType();
                        var typedArray = Array.CreateInstance(actualType, resultsList.Count);
                        for (int i = 0; i < resultsList.Count; i++)
                        {
                            typedArray.SetValue(resultsList[i], i);
                        }
                        this.DataSource = typedArray;
                    }
                    else
                    {
                        this.DataSource = resultsList;
                        ResetNativeSelection();
                    }
                    // -------------------------------------------------------------

                    // 🔴 بازگرداندن DisplayMember و ValueMember 'بعد' از مقداردهی دیتاسورس
                    if (!string.IsNullOrEmpty(savedDisplayMember))
                        this.DisplayMember = savedDisplayMember;

                    if (!string.IsNullOrEmpty(savedValueMember))
                        this.ValueMember = savedValueMember;

                    this.Text = currentText;

                    try { this.SelectionStart = currentSelectionStart; } catch { }

                    _isUpdatingText = false;

                    if (this.Items.Count > 0)
                    {
                        this.DroppedDown = true;
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        this.DroppedDown = false;
                    }
                }
            }
        }

        #endregion
    }
}