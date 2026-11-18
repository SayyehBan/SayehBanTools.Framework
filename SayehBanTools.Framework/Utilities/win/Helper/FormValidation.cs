using SayehBanTools.Framework.Converter;
using SayehBanTools.Framework.Utilities.Constants;
using SayehBanTools.Framework.Utilities.win.component.CommonControls;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// قواعد مشترک اعتبارسنجی فرم‌ها تا همه فرم‌ها یکسان رفتار کنند
    /// (در متد ValidateFormInputs هر فرم استفاده می‌شود)
    /// </summary>
    public static class FormValidation
    {
        /// <summary>حداقل طول رمز عبور جدید</summary>
        public const int MinPasswordLength = 4;

        /// <summary>
        /// آیا گزینه کامبوباکس واقعاً از لیست انتخاب شده است؟
        /// (متن تایپ‌شده باید دقیقاً متن همان آیتم انتخاب‌شده باشد؛ کامبوهای جستجوی «شامل» با متن اشتباه، ایندکس قبلی را نگه می‌دارند)
        /// </summary>
        public static bool IsValidComboSelection(ComboBox cmb)
        {
            if (cmb == null) return false;

            var dgvCombo = cmb as cmbboxDGV;
            if (dgvCombo != null) return dgvCombo.HasValidSelection();

            try
            {
                int index = cmb.SelectedIndex;
                if (index < 0 || index >= cmb.Items.Count || cmb.SelectedValue == null) return false;
                if (cmb.DropDownStyle == ComboBoxStyle.DropDownList) return true;

                string itemText = (cmb.GetItemText(cmb.Items[index]) ?? string.Empty).Trim();
                string typed = (cmb.Text ?? string.Empty).Trim();
                return string.Equals(itemText, typed, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        public static void RequireCombo(StringBuilder errors, ComboBox cmb, string title)
        {
            string typed;
            try { typed = cmb.Text; } catch { typed = string.Empty; }

            if (string.IsNullOrWhiteSpace(typed))
                errors.AppendLine($"- لطفاً {title} را انتخاب کنید.");
            else if (!IsValidComboSelection(cmb))
                errors.AppendLine($"- {title} «{typed.Trim()}» اشتباه می‌باشد. لطفاً از لیست انتخاب نمایید.");
        }

        public static void RequireText(StringBuilder errors, string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value)) errors.AppendLine("- " + message);
        }

        public static void ValidatePhone(StringBuilder errors, string phone, bool required = true)
        {
            string p = (phone ?? string.Empty).CleanValue().Trim();
            if (string.IsNullOrWhiteSpace(p))
            {
                if (required) errors.AppendLine("- لطفاً شماره تماس را وارد کنید.");
                return;
            }

            if (p.Length < StaticConstants.LimitTel || !p.StartsWith("0") || !p.All(char.IsDigit))
                errors.AppendLine("- لطفاً شماره تماس را صحیح وارد فرمائید (با 0 شروع شود و حداقل " + StaticConstants.LimitTel + " رقم باشد).");
        }

        public static void ValidateUsername(StringBuilder errors, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                errors.AppendLine("- لطفاً نام کاربری را وارد کنید.");
                return;
            }
            if (username.Trim().Any(char.IsWhiteSpace))
                errors.AppendLine("- نام کاربری نباید فاصله داشته باشد.");
        }

        /// <param name="required">در فرم ویرایش، خالی گذاشتن رمز یعنی «بدون تغییر»</param>
        public static void ValidatePassword(StringBuilder errors, string password, string confirm, bool required = true)
        {
            if (string.IsNullOrEmpty(password))
            {
                if (required) errors.AppendLine("- لطفاً رمز عبور را وارد کنید.");
                return;
            }
            if (password.Length < MinPasswordLength)
                errors.AppendLine($"- رمز عبور باید حداقل {MinPasswordLength} کاراکتر باشد.");
            if (password != confirm)
                errors.AppendLine("- رمز عبور و تکرار رمز عبور باهم هماهنگ نیست.");
        }

        public static void RequireImage(StringBuilder errors, PictureBox pic, string message)
        {
            if (pic == null || pic.Image == null) errors.AppendLine("- " + message);
        }

        public static void ShowErrors(StringBuilder errors)
        {
            MessageBox.Show(errors.ToString().TrimEnd(), "خطای اعتبارسنجی", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
        }
    }
}