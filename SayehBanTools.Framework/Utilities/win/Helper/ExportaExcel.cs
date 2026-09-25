using SayehBanTools.Framework.Converter;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SayehBanTools.Framework.Utilities.win.Helper
{
    public class ExportaExcel
    {
        /// <summary>
        /// گرفتن خروجی اکسل سریع از DataTable
        /// </summary>
        /// <param name="dtexpo">جدول داده ورودی</param>
        public void Exporta(DataTable dtexpo)
        {
            if (dtexpo == null || dtexpo.Rows.Count == 0)
            {
                MessageBox.Show("هیچ داده‌ای برای خروجی وجود ندارد.", "هشدار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                // استفاده از مسیر استاندارد MyDocuments سیستم
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Title = "مسیر ذخیره فایل اکسل را انتخاب کنید";
                dialog.Filter = "Excel Files (*.xls)|*.xls|All files (*.*)|*.*";
                dialog.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xls";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // استفاده از using جهت آزادکننده خودکار منابع فایل
                        using (StreamWriter wr = new StreamWriter(dialog.FileName, false, Encoding.Unicode))
                        {
                            // 1. نوشتن هدر ستون‌ها
                            for (int i = 0; i < dtexpo.Columns.Count; i++)
                            {
                                string colName = dtexpo.Columns[i].ColumnName.CleanValue();
                                wr.Write(colName + "\t");
                            }
                            wr.WriteLine();

                            // 2. نوشتن مقادیر سطرها
                            for (int i = 0; i < dtexpo.Rows.Count; i++)
                            {
                                for (int j = 0; j < dtexpo.Columns.Count; j++)
                                {
                                    object cellValue = dtexpo.Rows[i][j];
                                    if (cellValue != null && cellValue != DBNull.Value)
                                    {
                                        wr.Write(cellValue.ToString().CleanValue() + "\t");
                                    }
                                    else
                                    {
                                        wr.Write("\t");
                                    }
                                }
                                wr.WriteLine();
                            }
                        }

                        // پرسش از کاربر برای باز کردن خودکار فایل
                        DialogResult result = MessageBox.Show(
                            "خروجی اکسل با موفقیت انجام شد.\nآیا می‌خواهید فایل باز شود؟",
                            "موفقیت",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطا در ایجاد فایل اکسل: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

  
    }
}