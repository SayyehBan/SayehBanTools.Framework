using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel; // استفاده از Alias جهت جلوگیری از تداخل نام‌ها

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class DGVPagin : DataGridView
    {
        public bool GONextCell { get; set; }

        // مشخصات صفحه‌بندی
        [Browsable(false)]
        public int PageSize { get; set; } = 100;

        [Browsable(false)]
        public int CurrentPage { get; private set; } = 1;

        [Browsable(false)]
        public int TotalRecords { get; private set; } = 0;

        [Browsable(false)]
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // رویداد درخواست دریافت داده‌های صفحه جدید از سرویس/دیتابیس
        public event EventHandler<PageChangedEventArgs> PageChanged;

        private Panel _pagingPanel;
        private Label _lblPageInfo;
        private Button _btnFirst, _btnPrev, _btnNext, _btnLast;

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
                Height = 35,
                Dock = DockStyle.Bottom,
                Visible = false,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            _lblPageInfo = new Label
            {
                AutoSize = true,
                Location = new Point(15, 8),
                Font = new System.Drawing.Font("Tahoma", 9F, FontStyle.Regular)
            };

            _btnFirst = CreatePageButton("<<", (s, e) => GoToPage(1));
            _btnPrev = CreatePageButton("<", (s, e) => GoToPage(CurrentPage - 1));
            _btnNext = CreatePageButton(">", (s, e) => GoToPage(CurrentPage + 1));
            _btnLast = CreatePageButton(">>", (s, e) => GoToPage(TotalPages));

            _pagingPanel.Controls.Add(_lblPageInfo);
            _pagingPanel.Controls.Add(_btnLast);
            _pagingPanel.Controls.Add(_btnNext);
            _pagingPanel.Controls.Add(_btnPrev);
            _pagingPanel.Controls.Add(_btnFirst);
        }

        private Button CreatePageButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = 35,
                Height = 25,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(2)
            };
            btn.Click += onClick;
            return btn;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent != null && !_pagingPanel.IsDisposed)
            {
                this.Parent.Controls.Add(_pagingPanel);
                _pagingPanel.BringToFront();

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

            // شرط نمایش نوار Paging: تنها اگر رکوردهای کل بیشتر از ۱۰۰ (یا PageSize) باشد
            if (TotalRecords > PageSize)
            {
                _pagingPanel.Visible = true;
                _lblPageInfo.Text = $"صفحه {CurrentPage} از {TotalPages} (کل رکوردها: {TotalRecords})";

                _btnFirst.Enabled = CurrentPage > 1;
                _btnPrev.Enabled = CurrentPage > 1;
                _btnNext.Enabled = CurrentPage < TotalPages;
                _btnLast.Enabled = CurrentPage < TotalPages;

                // تنظیم موقعیت دکمه‌ها (راست به چپ)
                int rightMargin = _pagingPanel.Width - 40;
                _btnLast.Location = new Point(rightMargin, 5);
                _btnNext.Location = new Point(rightMargin - 40, 5);
                _btnPrev.Location = new Point(rightMargin - 80, 5);
                _btnFirst.Location = new Point(rightMargin - 120, 5);
            }
            else
            {
                _pagingPanel.Visible = false;
            }
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