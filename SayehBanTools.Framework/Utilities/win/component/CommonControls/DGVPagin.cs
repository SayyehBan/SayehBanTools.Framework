using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class DGVPagin : DataGridView
    {
        public bool GONextCell { get; set; }

        [Browsable(false)]
        public int PageSize { get; set; } = 10;

        [Browsable(false)]
        public int CurrentPage { get; private set; } = 1;

        [Browsable(false)]
        public int TotalRecords { get; private set; } = 0;

        [Browsable(false)]
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 1;

        public event EventHandler<PageChangedEventArgs> PageChanged;

        private Panel _pagingPanel;
        private Label _lblPageInfo;
        private ComboBox _cmbPageSize;
        private FlowLayoutPanel _pnlPageButtons;
        private Btn _btnFirst, _btnPrev, _btnNext, _btnLast;
        private bool _isUpdatingCmb = false;

        // برای جلوگیری از rebuild غیرضروری دکمه‌ها (رفع چشمک‌زدن و مخفی‌شدن ۱-۲ ثانیه‌ای)
        private int _lastRenderedFirstPage = -1;
        private int _lastRenderedLastPage = -1;
        private int _lastRenderedTotal = -1;

        // استایل دکمه‌های صفحه‌بندی — از Properties قابل تنظیم است
        private Color _pageButtonBackColor = Color.Black;
        private Color _pageButtonForeColor = Color.White;
        private Color _pageButtonBorderColor = Color.Gray;
        private Color _currentPageBackColor = Color.LightSteelBlue;
        private Color _currentPageForeColor = Color.White;
        private Color _disabledButtonBackColor = Color.FromArgb(210, 210, 210);
        private Color _disabledButtonForeColor = Color.FromArgb(120, 120, 120);
        private Font _pageButtonFont = new Font("Tahoma", 8F);
        private bool _styleInitialized = false;

        // متغیرهای مربوط به چک‌باکس انتخاب همه (Select All)
        private CheckBox _headerCheckBox;
        private string _selectAllColumnName = "";

        #region استایل دکمه‌های صفحه‌بندی (قابل تنظیم از Properties)

        /// <summary>رنگ پس‌زمینه دکمه‌های صفحه‌بندی</summary>
        [Category("Paging Style"), Description("رنگ پس‌زمینه دکمه‌های شماره صفحه")]
        public Color PageButtonBackColor
        {
            get { return _pageButtonBackColor; }
            set { _pageButtonBackColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ متن دکمه‌های صفحه‌بندی</summary>
        [Category("Paging Style"), Description("رنگ متن دکمه‌های شماره صفحه")]
        public Color PageButtonForeColor
        {
            get { return _pageButtonForeColor; }
            set { _pageButtonForeColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ خط دور دکمه‌های صفحه‌بندی</summary>
        [Category("Paging Style"), Description("رنگ خط دور دکمه‌های صفحه‌بندی")]
        public Color PageButtonBorderColor
        {
            get { return _pageButtonBorderColor; }
            set { _pageButtonBorderColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ پس‌زمینه دکمه صفحه جاری</summary>
        [Category("Paging Style"), Description("رنگ پس‌زمینه دکمه صفحه جاری")]
        public Color CurrentPageBackColor
        {
            get { return _currentPageBackColor; }
            set { _currentPageBackColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ متن دکمه صفحه جاری</summary>
        [Category("Paging Style"), Description("رنگ متن دکمه صفحه جاری")]
        public Color CurrentPageForeColor
        {
            get { return _currentPageForeColor; }
            set { _currentPageForeColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ پس‌زمینه دکمه‌های غیرفعال (اول/قبلی/بعدی/آخر وقتی به ابتدا یا انتهای لیست رسیده‌ایم)</summary>
        [Category("Paging Style"), Description("رنگ پس‌زمینه دکمه‌های غیرفعال")]
        public Color DisabledButtonBackColor
        {
            get { return _disabledButtonBackColor; }
            set { _disabledButtonBackColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ متن دکمه‌های غیرفعال</summary>
        [Category("Paging Style"), Description("رنگ متن دکمه‌های غیرفعال")]
        public Color DisabledButtonForeColor
        {
            get { return _disabledButtonForeColor; }
            set { _disabledButtonForeColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>فونت دکمه‌های صفحه‌بندی</summary>
        [Category("Paging Style"), Description("فونت دکمه‌های صفحه‌بندی")]
        public Font PageButtonFont
        {
            get { return _pageButtonFont; }
            set
            {
                _pageButtonFont = value ?? new Font("Tahoma", 8F);
                _lastRenderedFirstPage = -1; // ساخت مجدد دکمه‌ها با فونت جدید
                if (_styleInitialized && TotalRecords > 0) SetPagingInfo(TotalRecords, CurrentPage);
            }
        }

        /// <summary>رنگ پس‌زمینه دکمه‌های ناوبری (اول/قبلی/بعدی/آخر)</summary>
        [Category("Paging Style"), Description("رنگ پس‌زمینه دکمه‌های ناوبری (اول/قبلی/بعدی/آخر)")]
        public Color NavButtonBackColor
        {
            get { return _pageButtonBackColor; }
            set { _pageButtonBackColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        /// <summary>رنگ متن دکمه‌های ناوبری</summary>
        [Category("Paging Style"), Description("رنگ متن دکمه‌های ناوبری (اول/قبلی/بعدی/آخر)")]
        public Color NavButtonForeColor
        {
            get { return _pageButtonForeColor; }
            set { _pageButtonForeColor = value; if (_styleInitialized) RefreshPageButtonsState(); }
        }

        #endregion

        public DGVPagin()
        {
            DoubleBuffered = true;
            this.AllowUserToOrderColumns = true;
            InitializePagingControls();
            _styleInitialized = true;
        }

        #region چک‌باکس یکپارچه هدر (Select All)

        /// <summary>
        /// با فراخوانی این متد، یک چک‌باکس اصلی در هدر ستون موردنظر برای انتخاب همه قرار می‌گیرد
        /// </summary>
        /// <param name="checkboxColumnName">نام ستون چک‌باکس در گرید (مثلاً colSelect)</param>
        public void EnableSelectAllCheckbox(string checkboxColumnName = "colSelect")
        {
            _selectAllColumnName = checkboxColumnName;

            if (_headerCheckBox == null)
            {
                _headerCheckBox = new CheckBox
                {
                    Size = new Size(15, 15),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    TabStop = false
                };

                this.Controls.Add(_headerCheckBox);

                // مدیریت رویداد کلیک روی چک‌باکس اصلی
                _headerCheckBox.CheckedChanged += MasterCheckBox_CheckedChanged;

                // مدیریت تغییر موقعیت هدر
                this.ColumnWidthChanged += (s, e) => UpdateHeaderCheckBoxPosition();
                this.Scroll += (s, e) => UpdateHeaderCheckBoxPosition();
                this.Paint += (s, e) => UpdateHeaderCheckBoxPosition();
                this.SizeChanged += (s, e) => UpdateHeaderCheckBoxPosition();

                // در زمان بایند شدن دیتا، چک باکس خاموش شود
                this.DataBindingComplete += (s, e) =>
                {
                    _headerCheckBox.CheckedChanged -= MasterCheckBox_CheckedChanged;
                    _headerCheckBox.Checked = false;
                    _headerCheckBox.CheckedChanged += MasterCheckBox_CheckedChanged;
                    UpdateHeaderCheckBoxPosition();
                };

                // بروزرسانی چک‌باکس اصلی در صورتی که کاربر چک‌باکس‌های تکی را کلیک کند
                this.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (this.IsCurrentCellDirty && this.CurrentCell.OwningColumn.Name == _selectAllColumnName)
                    {
                        this.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    }
                };

                this.CellValueChanged += (s, e) =>
                {
                    // اضافه شدن شرط e.ColumnIndex >= 0 برای جلوگیری از خطای تغییر هدر ردیف
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && this.Columns[e.ColumnIndex].Name == _selectAllColumnName)
                    {
                        CheckMasterCheckBoxState();
                    }
                };
            }
        }
        private void MasterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.EndEdit();
            bool isChecked = _headerCheckBox.Checked;
            foreach (DataGridViewRow row in this.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells[_selectAllColumnName].Value = isChecked;
                }
            }
        }

        private void CheckMasterCheckBoxState()
        {
            if (_headerCheckBox == null || this.Rows.Count == 0) return;

            bool allChecked = true;
            foreach (DataGridViewRow row in this.Rows)
            {
                if (!row.IsNewRow)
                {
                    object val = row.Cells[_selectAllColumnName].Value;
                    if (val == null || !(bool)val)
                    {
                        allChecked = false;
                        break;
                    }
                }
            }

            // غیرفعال کردن موقت رویداد برای جلوگیری از لوپ
            _headerCheckBox.CheckedChanged -= MasterCheckBox_CheckedChanged;
            _headerCheckBox.Checked = allChecked;
            _headerCheckBox.CheckedChanged += MasterCheckBox_CheckedChanged;
        }

        private void UpdateHeaderCheckBoxPosition()
        {
            if (_headerCheckBox != null && this.Columns.Contains(_selectAllColumnName))
            {
                Rectangle rect = this.GetCellDisplayRectangle(this.Columns[_selectAllColumnName].Index, -1, true);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    // قرار دادن چک باکس دقیقاً در وسط سلول هدر
                    _headerCheckBox.Location = new Point(rect.Location.X + (rect.Width - _headerCheckBox.Width) / 2,
                                                         rect.Location.Y + (rect.Height - _headerCheckBox.Height) / 2);
                    _headerCheckBox.Visible = true;
                    _headerCheckBox.BringToFront();
                }
                else
                {
                    _headerCheckBox.Visible = false;
                }
            }
        }

        #endregion

        #region ساخت نوار ابزار Paging

        private void InitializePagingControls()
        {
            _pagingPanel = new PanelGlass
            {
                Height = 38,
                Dock = DockStyle.Bottom,
                Visible = false,
                BackColor = Color.FromArgb(240, 240, 240),
                RightToLeft = RightToLeft.Yes
            };

            _lblPageInfo = new Label
            {
                AutoSize = true,
                Location = new Point(10, 10),
                Font = new Font("Tahoma", 8.5F, FontStyle.Regular)
            };

            _cmbPageSize = new cmbbox
            {
                Cursor = Cursors.Hand,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 65,
                Location = new Point(220, 7),
                Font = new Font("Tahoma", 8.5F, FontStyle.Regular)
            };
            _cmbPageSize.Items.AddRange(new object[] { 10, 20, 50, 80, 100, 150, 200 });
            _cmbPageSize.SelectedItem = 10;
            _cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;

            _pnlPageButtons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Location = new Point(295, 4),
                WrapContents = false
            };

            _pagingPanel.Controls.Add(_lblPageInfo);
            _pagingPanel.Controls.Add(_cmbPageSize);
            _pagingPanel.Controls.Add(_pnlPageButtons);
        }

        private void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingCmb) return;

            if (_cmbPageSize.SelectedItem != null && int.TryParse(_cmbPageSize.SelectedItem.ToString(), out int newSize))
            {
                PageSize = newSize;
                CurrentPage = 1;
                PageChanged?.Invoke(this, new PageChangedEventArgs { PageNumber = CurrentPage, PageSize = PageSize });
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent != null && !_pagingPanel.IsDisposed)
            {
                this.Dock = DockStyle.Fill;

                if (!this.Parent.Controls.Contains(_pagingPanel))
                {
                    this.Parent.Controls.Add(_pagingPanel);
                    _pagingPanel.SendToBack();
                    this.BringToFront();
                }

                Form parentForm = this.FindForm();
                if (parentForm != null)
                {
                    parentForm.FormClosing += (s, args) => SaveColumnOrders();
                }
            }
        }

        public void SetPagingInfo(int totalRecords, int currentPage = 1)
        {
            this.TotalRecords = totalRecords;
            this.CurrentPage = currentPage;

            _isUpdatingCmb = true;
            if (_cmbPageSize.Items.Contains(PageSize))
                _cmbPageSize.SelectedItem = PageSize;
            else
                _cmbPageSize.SelectedItem = 10;
            _isUpdatingCmb = false;

            if (TotalRecords > PageSize)
            {
                // پنل فقط وقتی لازم است نمایش داده شود؛ هرگز هنگام رفرش مخفی نمی‌شود (رفع مخفی‌شدن ۱-۲ ثانیه‌ای)
                _pagingPanel.Visible = true;
                _lblPageInfo.Text = $"صفحه {CurrentPage} از {TotalPages} (کل: {TotalRecords})";

                // فقط وقتی ساختار صفحات عوض شد (تعداد کل یا بازه شماره‌ها) دکمه‌ها از نو ساخته می‌شوند
                int startPage, endPage;
                CalcRenderedRange(out startPage, out endPage);

                bool structureChanged =
                    _lastRenderedFirstPage != startPage ||
                    _lastRenderedLastPage != endPage ||
                    _lastRenderedTotal != TotalRecords;

                if (structureChanged || _pnlPageButtons.Controls.Count == 0)
                {
                    RenderPageButtons(startPage, endPage);
                }
                else
                {
                    // دکمه‌ها سر جایشان هستند؛ فقط رنگ/فونت/فعال‌بودنشان به‌روز می‌شود (بدون چشمک)
                    RefreshPageButtonsState();
                }
            }
            else
            {
                _pagingPanel.Visible = false;
            }
        }

        private void CalcRenderedRange(out int startPage, out int endPage)
        {
            int maxButtons = 10;
            int total = TotalPages;
            int current = CurrentPage;

            startPage = Math.Max(1, current - (maxButtons / 2));
            endPage = startPage + maxButtons - 1;

            if (endPage > total)
            {
                endPage = total;
                startPage = Math.Max(1, endPage - maxButtons + 1);
            }
        }

        /// <summary>
        /// بروزرسانی رنگ/فونت/فعال‌بودن دکمه‌های موجود بدون rebuild — بدون چشمک زدن
        /// </summary>
        private void RefreshPageButtonsState()
        {
            int current = CurrentPage;
            int total = TotalPages;

            // دکمه‌های ناوبری
            if (_btnFirst != null && _btnPrev != null)
            {
                bool back = current > 1;
                if (_btnFirst.Enabled != back) _btnFirst.Enabled = back;
                if (_btnPrev.Enabled != back) _btnPrev.Enabled = back;
            }
            if (_btnNext != null && _btnLast != null)
            {
                bool fwd = current < total;
                if (_btnNext.Enabled != fwd) _btnNext.Enabled = fwd;
                if (_btnLast.Enabled != fwd) _btnLast.Enabled = fwd;
            }

            // دکمه‌های شماره صفحه
            foreach (Control c in _pnlPageButtons.Controls)
            {
                var btn = c as Btn;
                if (btn == null) continue;

                btn.BorderColor = _pageButtonBorderColor;
                btn.DisabledBackColor = _disabledButtonBackColor;
                btn.DisabledForeColor = _disabledButtonForeColor;

                int pageNum;
                if (int.TryParse(btn.Text, out pageNum))
                {
                    bool isCurrent = pageNum == current;
                    bool shouldBeEnabled = !isCurrent;
                    if (btn.Enabled != shouldBeEnabled) btn.Enabled = shouldBeEnabled;
                    btn.BackColor = isCurrent ? _currentPageBackColor : _pageButtonBackColor;
                    btn.ForeColor = isCurrent ? _currentPageForeColor : _pageButtonForeColor;
                    btn.Font = isCurrent ? new Font(_pageButtonFont, FontStyle.Bold) : _pageButtonFont;
                    btn.Cursor = isCurrent ? Cursors.Default : Cursors.Hand;
                    // رنگ دکمه صفحه جاری در حالت disabled — از DisabledBackColor/ForeColor می‌آید
                    if (isCurrent)
                    {
                        btn.DisabledBackColor = _currentPageBackColor;
                        btn.DisabledForeColor = _currentPageForeColor;
                    }
                }
            }
        }

        private void RenderPageButtons(int startPage, int endPage)
        {
            // جدا کردن event handler ها قبل از Clear تا دوباره subscribe نشوند
            foreach (Control c in _pnlPageButtons.Controls)
            {
                var b = c as Btn;
                if (b != null) b.Click -= GoToPageClick;
            }
            _pnlPageButtons.Controls.Clear();

            int total = TotalPages;
            int current = CurrentPage;

            // ثبت بازه رندرشده برای تشخیص تغییر ساختار در دفعات بعد
            _lastRenderedFirstPage = startPage;
            _lastRenderedLastPage = endPage;
            _lastRenderedTotal = TotalRecords;

            _btnFirst = CreateNavButton("<<", "first", current > 1);
            _pnlPageButtons.Controls.Add(_btnFirst);

            _btnPrev = CreateNavButton("<", "prev", current > 1);
            _pnlPageButtons.Controls.Add(_btnPrev);

            for (int p = startPage; p <= endPage; p++)
            {
                int pageNum = p;
                var btnNum = new Btn
                {
                    Text = pageNum.ToString(),
                    Width = 32,
                    Height = 27,
                    Margin = new Padding(1),
                    Font = _pageButtonFont,
                    BackColor = pageNum == current ? _currentPageBackColor : _pageButtonBackColor,
                    ForeColor = pageNum == current ? _currentPageForeColor : _pageButtonForeColor,
                    BorderColor = _pageButtonBorderColor,
                    Cursor = Cursors.Hand,
                    Tag = pageNum // صفحه مقصد — در GoToPageClick استفاده می‌شود
                };

                if (pageNum == current)
                {
                    btnNum.Enabled = false;
                    btnNum.Cursor = Cursors.Default;
                    btnNum.Font = new Font(_pageButtonFont, FontStyle.Bold);
                    btnNum.DisabledBackColor = _currentPageBackColor;
                    btnNum.DisabledForeColor = _currentPageForeColor;
                }
                else
                {
                    btnNum.Click += GoToPageClick;
                }

                _pnlPageButtons.Controls.Add(btnNum);
            }

            _btnNext = CreateNavButton(">", "next", current < total);
            _pnlPageButtons.Controls.Add(_btnNext);

            _btnLast = CreateNavButton(">>", "last", current < total);
            _pnlPageButtons.Controls.Add(_btnLast);
        }

        private void GoToPageClick(object sender, EventArgs e)
        {
            var btn = sender as Btn;
            if (btn == null) return;

            // دکمه‌های شماره صفحه — Tag عددیِ صفحه مقصد
            if (btn.Tag is int)
            {
                GoToPage((int)btn.Tag);
                return;
            }

            // دکمه‌های ناوبری — مقصد در لحظه کلیک محاسبه می‌شود (همیشه به‌روز)
            // این رفع باگ است: اگر Tag عددی بود (مثل prev که current+1 بود)، بعد از
            // تغییر صفحه دیگر به‌روز نمی‌شد و کلیک دوم کار نمی‌کرد
            string kind = btn.Tag as string;
            switch (kind)
            {
                case "first": GoToPage(1); break;
                case "last": GoToPage(TotalPages); break;
                case "prev": GoToPage(CurrentPage - 1); break;
                case "next": GoToPage(CurrentPage + 1); break;
            }
        }

        private Btn CreateNavButton(string text, string kind, bool enabled)
        {
            var btn = new Btn
            {
                Text = text,
                Width = 32,
                Height = 27,
                Margin = new Padding(1),
                Enabled = enabled,
                BackColor = _pageButtonBackColor,
                ForeColor = _pageButtonForeColor,
                BorderColor = _pageButtonBorderColor,
                Font = _pageButtonFont,
                Cursor = Cursors.Hand,
                Tag = kind // نوع ناوبری: first / prev / next / last — مقصد در لحظه کلیک محاسبه می‌شود
            };
            // رنگ دکمه غیرفعال — چون Btn در حالت disabled رنگ DisabledBackColor خودش را می‌گیرد
            btn.DisabledBackColor = _disabledButtonBackColor;
            btn.DisabledForeColor = _disabledButtonForeColor;
            btn.Click += GoToPageClick;
            return btn;
        }

        private void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages || page == CurrentPage) return;
            CurrentPage = page;
            PageChanged?.Invoke(this, new PageChangedEventArgs { PageNumber = CurrentPage, PageSize = PageSize });
        }

        #endregion

        #region حفظ و بازیابی ترتیب ستون‌ها

        private string GetSettingsKey()
        {
            Form parentForm = this.FindForm();
            string formName = parentForm != null ? parentForm.Name : "UnknownForm";
            return $"{formName}_{this.Name}_ColumnOrders";
        }

        public void SaveColumnOrders()
        {
            try
            {
                if (this.Columns.Count == 0) return;

                string key = GetSettingsKey();
                List<string> columnOrders = new List<string>();

                foreach (DataGridViewColumn col in this.Columns)
                {
                    columnOrders.Add($"{col.Name}:{col.DisplayIndex}");
                }

                if (Properties.Settings.Default.DGVColumnOrders == null)
                {
                    Properties.Settings.Default.DGVColumnOrders = new StringCollection();
                }

                StringCollection sc = Properties.Settings.Default.DGVColumnOrders;
                for (int i = sc.Count - 1; i >= 0; i--)
                {
                    if (sc[i].StartsWith(key + "="))
                    {
                        sc.RemoveAt(i);
                    }
                }

                string data = key + "=" + string.Join(";", columnOrders);
                sc.Add(data);
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        public void RestoreColumnOrders()
        {
            try
            {
                if (this.Columns.Count == 0 || Properties.Settings.Default.DGVColumnOrders == null) return;

                string key = GetSettingsKey();
                StringCollection sc = Properties.Settings.Default.DGVColumnOrders;

                string savedData = null;
                foreach (string item in sc)
                {
                    if (item.StartsWith(key + "="))
                    {
                        savedData = item.Substring(key.Length + 1);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(savedData)) return;

                string[] colPairs = savedData.Split(';');
                Dictionary<string, int> orders = new Dictionary<string, int>();

                foreach (string pair in colPairs)
                {
                    string[] parts = pair.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int displayIndex))
                    {
                        orders[parts[0]] = displayIndex;
                    }
                }

                foreach (DataGridViewColumn col in this.Columns)
                {
                    if (orders.ContainsKey(col.Name))
                    {
                        int targetIndex = orders[col.Name];
                        if (targetIndex >= 0 && targetIndex < this.Columns.Count)
                        {
                            col.DisplayIndex = targetIndex;
                        }
                    }
                }
            }
            catch { }
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);

            this.RowHeadersWidth = 41;
            for (int i = 0; i < this.Rows.Count; i++)
            {
                this.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            this.ClearSelection();
            this.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Control;

            RestoreColumnOrders();
        }

        #endregion

        #region مدیریت کلیدها و خروجی اکسل

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (GONextCell && keyData == Keys.Enter)
            {
                base.ProcessTabKey(Keys.Tab);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (GONextCell && e.KeyCode == Keys.Enter)
            {
                base.ProcessTabKey(Keys.Tab);
                return true;
            }
            return base.ProcessDataGridViewKey(e);
        }

        private Color _Alternating = Color.Yellow;
        public Color AlteraningColor
        {
            get => _Alternating;
            set
            {
                _Alternating = value;
                this.AlternatingRowsDefaultCellStyle.BackColor = this._Alternating;
            }
        }

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            base.OnEditingControlShowing(e);
            e.CellStyle.BackColor = Color.Pink;
        }

        protected override void OnColumnHeaderMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseDoubleClick(e);

            if (this.Rows.Count == 0) return;

            try
            {
                ExportToExcelSafe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("کتابخانه اکسل یافت نشد. لطفاً مطمئن شوید نرم افزار Microsoft Excel روی سیستم نصب است.\n\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExportToExcelSafe()
        {
            Type excelType = Type.GetTypeFromProgID("Excel.Application");

            if (excelType == null)
            {
                MessageBox.Show("نرم افزار اکسل روی سیستم شما یافت نشد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
                return;
            }

            dynamic application = Activator.CreateInstance(excelType);
            dynamic workbook = application.Workbooks.Add();
            dynamic activeSheet = workbook.ActiveSheet;
            activeSheet.Name = "Sheet1";

            int excelColIndex = 1;
            var sortedColumns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in this.Columns) sortedColumns.Add(col);
            sortedColumns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

            for (int col = 0; col < sortedColumns.Count; col++)
            {
                if (sortedColumns[col].Visible)
                {
                    activeSheet.Cells[1, excelColIndex] = sortedColumns[col].HeaderText;
                    excelColIndex++;
                }
            }

            for (int row = 0; row < this.Rows.Count; row++)
            {
                if (this.Rows[row].IsNewRow) continue;

                excelColIndex = 1;
                for (int col = 0; col < sortedColumns.Count; col++)
                {
                    if (sortedColumns[col].Visible)
                    {
                        var cellValue = this.Rows[row].Cells[sortedColumns[col].Index].Value;
                        activeSheet.Cells[row + 2, excelColIndex] = cellValue?.ToString() ?? string.Empty;
                        excelColIndex++;
                    }
                }
            }

            application.Visible = true;
        }
        #endregion
    }

    public class PageChangedEventArgs : EventArgs
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}