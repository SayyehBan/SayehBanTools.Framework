using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    public class SystemHelper
    {
        /// <summary>
        ///دکمه خروج
        /// </summary>
        public static void ExitApp()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        /// <summary>
        /// دکمه خروج برای دکمه ضربدر
        /// </summary>
        /// <returns></returns>
        public static bool ConfirmExit()
        {
            DialogResult dr = MessageBox.Show(
                "آیا مایل به خروج از برنامه هستید؟",
                "خروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2, // فوکوس پیش‌فرض روی No برای جلوگیری از خروج اشتباهی
                MessageBoxOptions.RtlReading);  // جهت راست‌به‌چپ برای متن فارسی

            return dr == DialogResult.Yes;
        }
    }
}
