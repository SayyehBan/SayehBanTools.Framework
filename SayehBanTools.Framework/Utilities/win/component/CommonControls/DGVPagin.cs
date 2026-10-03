using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class DGVPagin : DataGridView
    {
        public bool GONextCell { get; set; }

        [Browsable(false)]
        public int PageSize { get; set; } = 10; // پیش‌فرض ۱۰ رکورد

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
        private Button _btnFirst, _btnPrev, _btnNext, _btnLast;
        private bool _isUpdatingCmb = false;

        public DGVPagin()
        {
            DoubleBuffered = true;
            this.AllowUserToOrderColumns = true;
            InitializePagingControls();
        }

        #region ساخت نوار ابزار Paging

        private void InitializePagingControls()
        {
            _pagingPanel = new Panel
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

            // ساخت ComboBox انتخاب تعداد رکورد
            _cmbPageSize = new ComboBox
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

            // پنل دکمه‌های صفحات
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
                // جهت جلوگیری از هم‌پوشانی اسکرول‌‌بار، DataGridView و Panel را هم‌سطح مدیریت می‌کنیم
                this.Dock = DockStyle.Fill;

                if (!this.Parent.Controls.Contains(_pagingPanel))
                {
                    this.Parent.Controls.Add(_pagingPanel);
                    // ترتیب Docking: پنل در پایین قرار می‌گیرد و DGV کل فضای باقیمانده را پر می‌کند
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
                _pagingPanel.Visible = true;
                _lblPageInfo.Text = $"صفحه {CurrentPage} از {TotalPages} (کل: {TotalRecords})";

                RenderPageButtons();
            }
            else
            {
                _pagingPanel.Visible = false;
            }
        }

        private void RenderPageButtons()
        {
            _pnlPageButtons.Controls.Clear();

            int total = TotalPages;
            int current = CurrentPage;

            // دکمه ابتدا >>
            _btnFirst = CreateNavButton(">>", 1, current > 1);
            _pnlPageButtons.Controls.Add(_btnFirst);

            // دکمه قبلی >
            _btnPrev = CreateNavButton(">", current - 1, current > 1);
            _pnlPageButtons.Controls.Add(_btnPrev);

            // محاسبه ۱۰ شماره صفحه متوالی
            int maxButtons = 10;
            int startPage = Math.Max(1, current - (maxButtons / 2));
            int endPage = startPage + maxButtons - 1;

            if (endPage > total)
            {
                endPage = total;
                startPage = Math.Max(1, endPage - maxButtons + 1);
            }

            for (int p = startPage; p <= endPage; p++)
            {
                int pageNum = p;
                var btnNum = new Button
                {
                    Text = pageNum.ToString(),
                    Width = 32,
                    Height = 27,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(1),
                    Font = new Font("Tahoma", 8F, pageNum == current ? FontStyle.Bold : FontStyle.Regular),
                    BackColor = pageNum == current ? Color.LightSteelBlue : Color.White
                };
                btnNum.FlatAppearance.BorderSize = 1;
                btnNum.FlatAppearance.BorderColor = Color.Gray;

                if (pageNum == current)
                {
                    btnNum.Enabled = false;
                }
                else
                {
                    btnNum.Click += (s, e) => GoToPage(pageNum);
                }

                _pnlPageButtons.Controls.Add(btnNum);
            }

            // دکمه بعدی <
            _btnNext = CreateNavButton("<", current + 1, current < total);
            _pnlPageButtons.Controls.Add(_btnNext);

            // دکمه انتها <<
            _btnLast = CreateNavButton("<<", total, current < total);
            _pnlPageButtons.Controls.Add(_btnLast);
        }

        private Button CreateNavButton(string text, int targetPage, bool enabled)
        {
            var btn = new Button
            {
                Text = text,
                Width = 32,
                Height = 27,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(1),
                Enabled = enabled
            };
            btn.Click += (s, e) => GoToPage(targetPage);
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

            try
            {
                if (this.Rows.Count == 0) return;

                Excel._Application application = new Excel.Application();
                Excel._Workbook workbook = application.Workbooks.Add(Type.Missing);
                Excel._Worksheet activeSheet = (Excel._Worksheet)workbook.ActiveSheet;
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
            catch (Exception ex)
            {
                MessageBox.Show("خطا در خروجی اکسل: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading);
            }
        }

        #endregion
    }

    public class PageChangedEventArgs : EventArgs
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}