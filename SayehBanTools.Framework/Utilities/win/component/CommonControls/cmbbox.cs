using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    /// <summary>
    /// ComboBox سفارشی:
    ///  - با رفتن موس روی آن (کادر متن، دکمه و لیست) نشانگر دست نمایش داده می‌شود
    ///  - پس‌زمینه سفید و نوشته سیاه
    ///  - AutoComplete با جستجوی «شامل» (وسط کلمه هم پیدا می‌شود) و چندکلمه‌ای
    ///  - سازگار با Items و DataSource (DisplayMember / ValueMember)
    ///  - نرمال‌سازی حروف فارسی و عربی (ي/ی ، ك/ک ، ارقام ، نیم‌فاصله)
    /// </summary>
    public class cmbbox : ComboBox
    {
        #region Win32 (نشانگر دست روی کادر متن و لیست)

        private const int WM_SETCURSOR = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COMBOBOXINFO
        {
            public int cbSize;
            public NativeRect rcItem;
            public NativeRect rcButton;
            public int stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;   // کادر متن (Edit)
            public IntPtr hwndList;   // لیست باز شونده
        }

        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>پنجره‌های داخلی ComboBox نشانگر خودشان را (I-Beam / فلش) تنظیم می‌کنند؛ اینجا دست می‌کنیم.</summary>
        private sealed class HandCursorWindow : NativeWindow
        {
            private readonly Control _owner;

            public HandCursorWindow(Control owner)
            {
                _owner = owner;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_SETCURSOR && _owner.Enabled)
                {
                    Cursor.Current = Cursors.Hand;
                    m.Result = (IntPtr)1;
                    return;
                }
                base.WndProc(ref m);
            }
        }

        private HandCursorWindow _editHook;
        private HandCursorWindow _listHook;

        #endregion

        #region فیلدها

        private Color _focusBackColor = Color.White;
        private Color _focusForeColor = Color.Black;
        private Font _focusFont;
        private bool _focusTextSelect = true;
        private bool _enterToTab = true;
        private bool _searchEnabled = true;
        private Color _highlightBackColor = Color.FromArgb(204, 232, 255);

        private bool _focusApplied;
        private Color _normalBackColor = Color.White;
        private Color _normalForeColor = Color.Black;
        private Font _normalFont;

        // وضعیت جستجو
        private bool _sessionActive;      // یک نشست جستجو فعال است (لیست فیلتر شده)
        private bool _filtering;          // در حال تغییر داخلی لیست هستیم؛ رویدادها ارسال نشوند
        private bool _openingByFilter;
        private bool _closingByFilter;
        private object _originalDataSource;
        private string _displayMember = string.Empty;
        private string _valueMember = string.Empty;
        private List<object> _snapshot;   // کپی کامل آیتم‌ها
        private List<string> _keys;       // متن نرمال‌شده هر آیتم
        private object _notifiedItem;     // آخرین آیتمی که رویداد انتخابش ارسال شده

        #endregion

        #region سازنده

        public cmbbox()
        {
            DropDownStyle = ComboBoxStyle.DropDown;   // برای تایپ و جستجو باید DropDown باشد
            DrawMode = DrawMode.OwnerDrawFixed;
            FlatStyle = FlatStyle.Standard;
            BackColor = Color.White;
            ForeColor = Color.Black;
            Cursor = Cursors.Hand;
            MaxDropDownItems = 10;
            ItemHeight = Font.Height + 6;

            // AutoComplete ویندوز خاموش می‌شود چون جستجوی خودمان جایگزین آن است
            AutoCompleteMode = AutoCompleteMode.None;
            AutoCompleteSource = AutoCompleteSource.None;
        }

        #endregion

        #region خواندن امن متن و آیتم انتخاب‌شده

        /// <summary>
        /// متن کادر را مستقیم از ویندوز می‌خواند. وقتی لیست خالی است، Text خود ComboBox
        /// ممکن است ArgumentOutOfRangeException بدهد (چون داخلش Items[SelectedIndex] را می‌خواند).
        /// </summary>
        private string RawText
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

        /// <summary>SelectedItem بدون خطا؛ اگر اندیس معتبر نباشد null برمی‌گرداند.</summary>
        private object SafeSelectedItem
        {
            get
            {
                int i = SelectedIndex;
                return (i >= 0 && i < Items.Count) ? Items[i] : null;
            }
        }

        #endregion

        #region ویژگی‌های عمومی

        [Category("Aramestan"), DefaultValue(true)]
        [Description("جستجوی «شامل» داخل آیتم‌ها هنگام تایپ (فقط در حالت DropDown)")]
        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            set { _searchEnabled = value; }
        }

        [Category("Aramestan"), DefaultValue(typeof(Color), "204, 232, 255")]
        [Description("رنگ پس‌زمینه آیتمی که موس یا کیبورد روی آن است")]
        public Color HighlightBackColor
        {
            get { return _highlightBackColor; }
            set { _highlightBackColor = value; Invalidate(); }
        }

        [Category("Aramestan"), DefaultValue(true)]
        [Description("با ورود به کنترل کل متن انتخاب شود")]
        public bool FocusTextSelect
        {
            get { return _focusTextSelect; }
            set { _focusTextSelect = value; }
        }

        [Category("Aramestan"), DefaultValue(typeof(Color), "White")]
        public Color FocusBackColor
        {
            get { return _focusBackColor; }
            set { _focusBackColor = value; }
        }

        [Category("Aramestan"), DefaultValue(typeof(Color), "Black")]
        public Color FocusForeColor
        {
            get { return _focusForeColor; }
            set { _focusForeColor = value; }
        }

        [Category("Aramestan"), DefaultValue(null)]
        [Description("اگر خالی باشد فونت هنگام فوکوس تغییر نمی‌کند")]
        public Font FocusFont
        {
            get { return _focusFont; }
            set { _focusFont = value; }
        }

        [Category("Aramestan"), DefaultValue(true)]
        [Description("با زدن Enter به کنترل بعدی برود")]
        public bool EnterToTab
        {
            get { return _enterToTab; }
            set { _enterToTab = value; }
        }

        #endregion

        #region ظاهر (رسم آیتم‌ها)

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ItemHeight = Font.Height + 6;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            bool inEditArea = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
            bool highlighted = !inEditArea && (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color back = highlighted ? _highlightBackColor : BackColor;
            Color fore = Enabled ? ForeColor : SystemColors.GrayText;

            using (SolidBrush brush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                                        TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

                if (RightToLeft == RightToLeft.Yes)
                    flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
                else
                    flags |= TextFormatFlags.Left;

                Rectangle textRect = Rectangle.Inflate(e.Bounds, -4, 0);
                TextRenderer.DrawText(e.Graphics, GetItemText(Items[e.Index]), e.Font, textRect, fore, flags);
            }

            if (inEditArea && (e.State & DrawItemState.Focus) == DrawItemState.Focus)
                e.DrawFocusRectangle();
        }

        #endregion

        #region Handle / نشانگر دست

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            HookChildWindows();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnhookChildWindows();
            base.OnHandleDestroyed(e);
        }

        private void HookChildWindows()
        {
            UnhookChildWindows();

            COMBOBOXINFO info = new COMBOBOXINFO();
            info.cbSize = Marshal.SizeOf(typeof(COMBOBOXINFO));
            if (!GetComboBoxInfo(Handle, ref info))
                return;

            if (info.hwndItem != IntPtr.Zero && info.hwndItem != Handle)
            {
                _editHook = new HandCursorWindow(this);
                _editHook.AssignHandle(info.hwndItem);
            }

            if (info.hwndList != IntPtr.Zero && info.hwndList != Handle)
            {
                _listHook = new HandCursorWindow(this);
                _listHook.AssignHandle(info.hwndList);
            }
        }

        private void UnhookChildWindows()
        {
            if (_editHook != null)
            {
                if (_editHook.Handle != IntPtr.Zero) _editHook.ReleaseHandle();
                _editHook = null;
            }
            if (_listHook != null)
            {
                if (_listHook.Handle != IntPtr.Zero) _listHook.ReleaseHandle();
                _listHook = null;
            }
        }

        #endregion

        #region فوکوس و کیبورد

        protected override void OnEnter(EventArgs e)
        {
            if (!_focusApplied)
            {
                _normalBackColor = BackColor;
                _normalForeColor = ForeColor;
                _normalFont = Font;
                _focusApplied = true;

                BackColor = _focusBackColor;
                ForeColor = _focusForeColor;
                if (_focusFont != null) Font = _focusFont;
            }

            base.OnEnter(e);

            if (_focusTextSelect && DropDownStyle != ComboBoxStyle.DropDownList && IsHandleCreated)
                BeginInvoke(new MethodInvoker(SelectAllText));
        }

        protected override void OnLeave(EventArgs e)
        {
            EndSession();

            if (_focusApplied)
            {
                _focusApplied = false;
                BackColor = _normalBackColor;
                ForeColor = _normalForeColor;
                if (_focusFont != null && _normalFont != null) Font = _normalFont;
            }

            base.OnLeave(e);
        }

        private void SelectAllText()
        {
            if (IsDisposed || !IsHandleCreated || !ContainsFocus) return;
            SelectionStart = 0;
            SelectionLength = RawText.Length;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Alt && !e.Control)
            {
                bool wasDropped = DroppedDown;

                if (wasDropped)
                {
                    // اگر چیزی هایلایت نشده، اولین نتیجه انتخاب شود
                    if (_sessionActive && SelectedIndex < 0 && Items.Count > 0)
                        SelectedIndex = 0;

                    DroppedDown = false;
                }

                if (wasDropped || _enterToTab)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    base.OnKeyDown(e);

                    if (_enterToTab && Parent != null)
                        Parent.SelectNextControl(this, true, true, true, true);
                    return;
                }
            }

            base.OnKeyDown(e);
        }

        #endregion

        #region AutoComplete: جستجوی «شامل» داخل آیتم‌ها

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);

            if (DesignMode || _filtering || !_searchEnabled ||
                DropDownStyle == ComboBoxStyle.DropDownList || !ContainsFocus)
                return;

            // تغییر متن ناشی از حرکت با کلیدهای بالا/پایین در لیست است، نه تایپ
            object selected = SafeSelectedItem;
            if (selected != null &&
                string.Equals(GetItemText(selected), RawText, StringComparison.Ordinal))
                return;

            ApplyFilter();
        }

        protected override void OnDropDown(EventArgs e)
        {
            // کاربر با دکمه فلش لیست کامل را خواسته
            if (_sessionActive && !_openingByFilter)
                EndSession();

            base.OnDropDown(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            if (_sessionActive && !_closingByFilter && IsHandleCreated)
                BeginInvoke(new MethodInvoker(EndSession));
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            // تغییر DataSource از بیرون؛ کپی قدیمی دیگر معتبر نیست
            if (!_filtering)
            {
                _sessionActive = false;
                _snapshot = null;
                _keys = null;
                _originalDataSource = null;
            }
            base.OnDataSourceChanged(e);
        }

        // در زمان فیلتر کردن، رویدادهای انتخاب (که ساختگی هستند) به کد برنامه نرسند
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (_filtering) return;
            base.OnSelectedIndexChanged(e);
            _notifiedItem = SafeSelectedItem;
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            if (_filtering) return;
            base.OnSelectedValueChanged(e);
        }

        private void BeginSession()
        {
            _notifiedItem = SafeSelectedItem;
            _originalDataSource = DataSource;

            _snapshot = new List<object>(Items.Count);
            foreach (object item in Items)
                _snapshot.Add(item);

            _keys = new List<string>(_snapshot.Count);
            foreach (object item in _snapshot)
                _keys.Add(Normalize(GetItemText(item)));

            // با DataSource نمی‌شود Items را تغییر داد؛ موقتاً به حالت Items می‌رویم
            if (_originalDataSource != null)
            {
                _displayMember = DisplayMember;
                _valueMember = ValueMember;
                DataSource = null;
                DisplayMember = _displayMember;
                ValueMember = _valueMember;
            }

            _sessionActive = true;
        }

        private void ApplyFilter()
        {
            string typed = RawText;
            int caret = SelectionStart;
            List<object> matches = new List<object>();
            _filtering = true;
            try
            {
                if (!_sessionActive)
                    BeginSession();

                string key = Normalize(typed).Trim();
                string[] tokens = key.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 0)
                {
                    matches.AddRange(_snapshot);
                }
                else
                {
                    List<object> startsWith = new List<object>();
                    List<object> contains = new List<object>();
                    for (int i = 0; i < _snapshot.Count; i++)
                    {
                        string k = _keys[i];
                        bool all = true;
                        for (int t = 0; t < tokens.Length; t++)
                        {
                            if (k.IndexOf(tokens[t], StringComparison.Ordinal) < 0)
                            {
                                all = false;
                                break;
                            }
                        }
                        if (!all) continue;
                        if (k.StartsWith(key, StringComparison.Ordinal))
                            startsWith.Add(_snapshot[i]);
                        else
                            contains.Add(_snapshot[i]);
                    }
                    matches.AddRange(startsWith);   // اول آنهایی که با عبارت شروع می‌شوند
                    matches.AddRange(contains);     // بعد بقیه نتایج
                }

                // ۱. اگر نتیجه‌ای نیست، اول لیست بازشونده را می‌بندیم تا خطای Index ندهد
                if (matches.Count == 0 && DroppedDown)
                {
                    _closingByFilter = true;
                    try { DroppedDown = false; }
                    finally { _closingByFilter = false; }
                }

                BeginUpdate();
                try
                {
                    // ۲. قبل از پاک کردن آیتم‌ها، SelectedIndex را -1 می‌کنیم تا WinForms خطای اندیس ندهد
                    SelectedIndex = -1;
                    Items.Clear();

                    if (matches.Count > 0)
                    {
                        Items.AddRange(matches.ToArray());
                    }
                }
                finally
                {
                    EndUpdate();
                }

                // بازگرداندن متن تایپ‌شده و وضعیت مکان‌نما
                if (RawText != typed) Text = typed;
                SelectionStart = Math.Min(caret, typed.Length);
                SelectionLength = 0;
            }
            finally
            {
                _filtering = false;
            }

            // ۳. اگر نتیجه‌ای یافت شد، لیست را باز می‌کنیم
            if (matches.Count > 0)
            {
                if (!DroppedDown)
                {
                    _openingByFilter = true;
                    try { DroppedDown = true; }
                    finally { _openingByFilter = false; }
                }
                Cursor.Current = Cursors.Hand;
            }

            // تنظیم مجدد مکان‌نما (چون باز/بسته شدن لیست Selection را تغییر می‌دهد)
            SelectionStart = Math.Min(caret, typed.Length);
            SelectionLength = 0;
        }

        private void EndSession()
        {
            if (!_sessionActive) return;
            _sessionActive = false;

            _filtering = true;
            try
            {
                string typed = RawText;
                int caret = SelectionStart;
                object chosen = SafeSelectedItem;

                // اگر بعد از انتخاب، متن دستی تغییر کرده، آن انتخاب معتبر نیست
                if (chosen != null &&
                    !string.Equals(GetItemText(chosen), typed, StringComparison.CurrentCultureIgnoreCase))
                    chosen = null;

                BeginUpdate();
                try
                {
                    Items.Clear();
                    if (_originalDataSource != null)
                        DataSource = _originalDataSource;
                    else
                        Items.AddRange(_snapshot.ToArray());
                }
                finally
                {
                    EndUpdate();
                }

                int index = -1;
                if (chosen != null)
                {
                    index = Items.IndexOf(chosen);
                    if (index < 0) index = FindStringExact(GetItemText(chosen));
                }

                SelectedIndex = index;

                if (index >= 0)
                {
                    if (DataManager != null && DataManager.Position != index)
                        DataManager.Position = index;
                }
                else
                {
                    Text = typed;
                    SelectionStart = Math.Min(caret, typed.Length);
                    SelectionLength = 0;
                }
            }
            finally
            {
                _filtering = false;
                _snapshot = null;
                _keys = null;
                _originalDataSource = null;
            }

            // اگر انتخاب نهایی با آخرین رویداد ارسال‌شده فرق دارد (مثلاً متن دقیقاً برابر یک آیتم تایپ شده)
            object current = SafeSelectedItem;
            if (!object.Equals(current, _notifiedItem))
            {
                _notifiedItem = current;
                OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        /// <summary>نرمال‌سازی برای مقایسه: حروف عربی به فارسی، ارقام به لاتین، حذف نیم‌فاصله/اعراب، حروف کوچک.</summary>
        private static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char ch in s)
            {
                char c = ch;

                if (c == '\u064A' || c == '\u0649') c = '\u06CC';                          // ي ى  ->  ی
                else if (c == '\u0643') c = '\u06A9';                                      // ك    ->  ک
                else if (c >= '\u06F0' && c <= '\u06F9') c = (char)('0' + (c - '\u06F0')); // ۰-۹ -> 0-9
                else if (c >= '\u0660' && c <= '\u0669') c = (char)('0' + (c - '\u0660')); // ٠-٩ -> 0-9
                else if (c == '\u200C' || c == '\u200B' || c == '\u0640' ||
                         (c >= '\u064B' && c <= '\u065F')) continue;                      // نیم‌فاصله، کشیده، اعراب

                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        #endregion
    }
}