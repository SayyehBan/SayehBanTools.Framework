using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    /// <summary>
    /// جا شدن فرم‌ها در صفحه‌های کوچک (مثل مانیتور 19 اینچ 1366×768):
    /// اگر فرم از ناحیه کاری صفحه (بدون نوار وظیفه) بزرگ‌تر باشد، کوچک می‌شود، اسکرول می‌گیرد و وسط صفحه قرار می‌گیرد.
    /// </summary>
    public static class ScreenFit
    {
        /// <summary>قلاب برنامه مصرف‌کننده (مثلاً سطح دسترسی AccessControl خود برنامه) که هنگام ثبت فرم با AddFittedForm صدا زده می‌شود</summary>
        public static Action<Form> FormAttached { get; set; }

        public static void Attach(Form form)
        {
            if (form == null) return;
            form.Load += (s, e) => Fit(form);
        }

        public static void Fit(Form form)
        {
            if (form == null || form.WindowState == FormWindowState.Maximized) return;

            Rectangle wa = Screen.FromControl(form).WorkingArea;
            int width = Math.Min(form.Width, wa.Width);
            int height = Math.Min(form.Height, wa.Height);

            if (width < form.Width || height < form.Height)
            {
                form.AutoScroll = true;
                form.Size = new Size(width, height);
            }

            // وسط ناحیه کاری (نه کل صفحه، تا زیر نوار وظیفه نرود)
            if (form.StartPosition == FormStartPosition.CenterScreen || width < form.Width || form.Bottom > wa.Bottom || form.Right > wa.Right)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(wa.Left + (wa.Width - form.Width) / 2, wa.Top + (wa.Height - form.Height) / 2);
            }
        }

        /// <summary>
        /// ثبت فرم در DI به همراه تنظیم خودکار اندازه با صفحه
        /// </summary>
        public static IServiceCollection AddFittedForm<TForm>(this IServiceCollection services) where TForm : Form
        {
            return services.AddTransient(sp =>
            {
                var form = ActivatorUtilities.CreateInstance<TForm>(sp);
                Attach(form);
                EnterNavigation.Attach(form);   // حرکت با Enter بین کنترل‌ها و خانه‌های جدول در همه فرم‌ها
                FormAttached?.Invoke(form);     // قلاب برنامه مصرف‌کننده (سطح دسترسی و ...)
                return form;
            });
        }
    }
}