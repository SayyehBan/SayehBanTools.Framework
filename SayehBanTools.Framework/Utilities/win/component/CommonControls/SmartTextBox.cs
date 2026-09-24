using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.component.CommonControls
{
    public class SmartTextBox : TextBox
    {
        public enum TargetInputType
        {
            Normal,         // هر نوع متنی
            CharacterOnly,  // فقط حروف
            Numeric,        // فقط عدد
            Money,          // عدد با جداکننده ۳ رقمی
            Date            // تاریخ هوشمند با Mask
        }

        private TargetInputType _inputType = TargetInputType.Normal;
        private bool _isGregorianDate = false;

        #region Properties

        [Category("Smart Properties")]
        [Description("نوع ورودی کادر متن را تعیین می‌کند.")]
        public TargetInputType InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                ApplyInputTypeSettings();
            }
        }

        [Category("Smart Properties")]
        [Description("اگر true باشد تاریخ میلادی (چپ‌چین) و اگر false باشد تاریخ شمسی (راست‌چین) تنظیم می‌شود.")]
        public bool IsGregorianDate
        {
            get => _isGregorianDate;
            set
            {
                _isGregorianDate = value;
                if (_inputType == TargetInputType.Date)
                {
                    ApplyDateDirectionAndFormatting();
                }
            }
        }

        [Browsable(false)]
        [Description("مقدار عددی بدون جداکننده (مخصوص حالت Money)")]
        public string UnformattedValue
        {
            get
            {
                if (_inputType == TargetInputType.Money)
                {
                    return Text.Replace(",", "").Replace("٫", "").Trim();
                }
                return Text;
            }
        }

        #endregion

        public SmartTextBox()
        {
            ApplyInputTypeSettings();
        }

        private void ApplyInputTypeSettings()
        {
            if (_inputType == TargetInputType.Date)
            {
                ApplyDateDirectionAndFormatting();
            }
            else
            {
                this.MaxLength = 32767;
            }
        }

        private void ApplyDateDirectionAndFormatting()
        {
            this.MaxLength = 10; // YYYY/MM/DD

            if (_isGregorianDate)
            {
                // تنظیمات جهت تایپ میلادی (چپ به راست)
                this.RightToLeft = RightToLeft.No;
                this.TextAlign = HorizontalAlignment.Left;
            }
            else
            {
                // تنظیمات جهت تایپ شمسی (راست به چپ)
                this.RightToLeft = RightToLeft.Yes;
                this.TextAlign = HorizontalAlignment.Right;
            }
        }

        #region Event Overrides

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // مجاز بودن کلیدهای کنترلی مثل Backspace
            if (char.IsControl(e.KeyChar)) return;

            switch (_inputType)
            {
                case TargetInputType.CharacterOnly:
                    if (char.IsDigit(e.KeyChar)) e.Handled = true;
                    break;

                case TargetInputType.Numeric:
                case TargetInputType.Money:
                    if (!char.IsDigit(e.KeyChar)) e.Handled = true;
                    break;

                case TargetInputType.Date:
                    // پذیرش عدد و کاراکتر اسلش
                    if (!char.IsDigit(e.KeyChar) && e.KeyChar != '/')
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            // ۱. حالت جداکننده سه رقمی برای Money
            if (_inputType == TargetInputType.Money)
            {
                string rawText = Text.Replace(",", "");
                if (decimal.TryParse(rawText, out decimal number))
                {
                    int selectionStart = SelectionStart;
                    int oldLength = Text.Length;

                    Text = string.Format("{0:N0}", number);

                    int newLength = Text.Length;
                    SelectionStart = Math.Max(0, selectionStart + (newLength - oldLength));
                }
                else if (string.IsNullOrEmpty(rawText))
                {
                    Text = string.Empty;
                }
            }
            // ۲. ماسک هوشمند تاریخ موقع تایپ
            else if (_inputType == TargetInputType.Date)
            {
                FormatDateInputLive();
            }
        }

        private void FormatDateInputLive()
        {
            // استخراج فقط اعداد
            string digits = new string(Text.Where(char.IsDigit).ToArray());

            if (digits.Length > 8)
            {
                digits = digits.Substring(0, 8);
            }

            string formatted = string.Empty;

            if (digits.Length <= 4)
            {
                formatted = digits;
            }
            else if (digits.Length <= 6)
            {
                formatted = $"{digits.Substring(0, 4)}/{digits.Substring(4)}";
            }
            else
            {
                formatted = $"{digits.Substring(0, 4)}/{digits.Substring(4, 2)}/{digits.Substring(6)}";
            }

            if (Text != formatted)
            {
                int cursorPosition = SelectionStart;
                Text = formatted;

                // تنظیم مجدد موقعیت مکان‌نما پس از درج اسلش
                SelectionStart = Text.Length;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            // اعتبارسنجی نهایی تاریخ هنگام خروج از کادر
            if (_inputType == TargetInputType.Date && !string.IsNullOrWhiteSpace(Text))
            {
                ValidateFinalDate();
            }
        }

        #endregion

        #region Date Validation

        private void ValidateFinalDate()
        {
            string digitsOnly = new string(Text.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 8)
            {
                string yearStr = digitsOnly.Substring(0, 4);
                string monthStr = digitsOnly.Substring(4, 2);
                string dayStr = digitsOnly.Substring(6, 2);

                int year = int.Parse(yearStr);
                int month = int.Parse(monthStr);
                int day = int.Parse(dayStr);

                if (!IsValidDate(year, month, day))
                {
                    MessageBox.Show("تاریخ وارد شده معتبر نیست!", "خطا در ورود تاریخ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Focus();
                    this.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("لطفاً تاریخ را به صورت ۸ رقمی وارد کنید (مثال: 14030101).", "تاریخ ناقص", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Focus();
                this.SelectAll();
            }
        }

        private bool IsValidDate(int year, int month, int day)
        {
            try
            {
                if (_isGregorianDate)
                {
                    DateTime dt = new DateTime(year, month, day);
                    return true;
                }
                else
                {
                    PersianCalendar pc = new PersianCalendar();
                    DateTime dt = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}