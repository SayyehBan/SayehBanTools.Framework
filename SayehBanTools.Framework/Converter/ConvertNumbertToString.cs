using System;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// تبدیل عدد به رشته
    /// </summary>
    public class ConvertNumbertToString
    {  //        this.toolTip1.SetToolTip(this.textBox1, ConvertRaghamToHorof(Int64.Parse( textBox1.Text)));
       //this.label1.Text = ConvertRaghamToHorof(Int64.Parse(textBox1.Text));


        #region ConvertRaghamToHorof
        /// <summary>
        /// عدد رادریافت وبه حروف تبدیل می کند 
        /// </summary>
        /// <param name="Ragham">رقم </param>
        /// <returns>به حروف عدد را برمی گرداند</returns>
        public static string ConvertRaghamToHorof(Int64 Ragham)
        {
            return ConvertRaghamToHorof(ConvertRaghamToJodaJoda(Ragham));
        }
        /// <summary>
        /// یک عدد به صورت سه رقم جدا را دریافت وبه حروف تبدیل می کند
        /// </summary>
        /// <param name="Ragham"></param>
        /// <returns></returns>
        public static string ConvertRaghamToHorof(string Ragham)
        {
            string horof = "";

            try
            {
                string[] AddJoda = Ragham.Split(',');
                for (int i = 0; i < AddJoda.Length; i++)
                {
                    //RadMessageBox.Show(AddJoda[i]);
                    string strh = pishConvertRaghamToHorof3(int.Parse(AddJoda[i]), AddJoda.Length - i);
                    if (horof == "")
                    {
                        horof = horof + strh;
                    }
                    else
                    {
                        if (strh != "") horof = horof + " و " + strh;
                    }

                }
                return horof;
            }
            catch
            {
                return "صفر";
            }
            finally
            {
            }

        }
        /// <summary>
        /// اعداد از صفر تا 19 را به حروف تبدیل میکند
        /// </summary>
        /// <param name="Ragham">عدد بین 0 تا 19</param>
        /// <returns>حروف را برمی گرداند در صورت خطا -1  و درصورت عدد نامعتبر تهی برمی گرداند</returns>
        private static string ConvertRaghamToHorof0to19(int Ragham)
        {
            try
            {
                //اعداد صفر  تا بیست
                if (Ragham > 19) return "";
                switch (Ragham)
                {
                    case 0:
                        return "صفر";
                    case 1:
                        return "یک ";
                    case 2:
                        return "دو";
                    case 3:
                        return "سه ";
                    case 4:
                        return "چهار";
                    case 5:
                        return "پنج ";
                    case 6:
                        return "شش ";
                    case 7:
                        return "هفت ";
                    case 8:
                        return "هشت ";
                    case 9:
                        return "نه ";
                    case 10:
                        return "ده ";
                    case 11:
                        return "یازده ";
                    case 12:
                        return "دوازده ";
                    case 13:
                        return "سیزده ";
                    case 14:
                        return "چهارده ";
                    case 15:
                        return "پانزده ";
                    case 16:
                        return "شانزده ";
                    case 17:
                        return "هیفده ";
                    case 18:
                        return "هیجده ";
                    case 19:
                        return "نوزده ";
                        //case 20:
                        //    return "بیست"; 

                }


                return "";
            }
            catch
            {
                return "-1";
            }
            finally
            {
            }

        }
        /// <summary>
        /// اعداد از بیست   تا نود نه را به حروف تبدیل میکند
        /// </summary>
        /// <param name="Ragham">عدد بین 20 تا 99</param>
        /// <returns>حروف را برمی گرداند در صورت خطا -1  و درصورت عدد نامعتبر تهی برمی گرداند</returns>
        private static string ConvertRaghamToHorof20to99(int Ragham)
        {
            try
            {
                //اعداد صفر  تا بیست
                if (!(Ragham > 19 && Ragham < 100)) return "";
                // برای تبدیل به حروف  رقم دوم
                string str = ConvertRaghamToHorof0to19(int.Parse(Ragham.ToString().Substring(1)));
                if (Ragham > 19 && Ragham < 30)
                {
                    if (Ragham == 20)
                    {
                        return "بیست";
                    }
                    else
                    {
                        return "بیست و" + str;
                    }

                }
                else if (Ragham > 29 && Ragham < 40)
                {
                    if (Ragham == 30)
                    {
                        return "سی";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "سی و" + str;
                    }
                }
                else if (Ragham > 39 && Ragham < 50)
                {
                    if (Ragham == 40)
                    {
                        return "چهل";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "چهل و" + str;
                    }
                }
                else if (Ragham > 49 && Ragham < 60)
                {
                    if (Ragham == 50)
                    {
                        return "پنجاه";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "پنجاه و" + str;
                    }
                }
                else if (Ragham > 59 && Ragham < 70)
                {
                    if (Ragham == 60)
                    {
                        return "شصت";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "شصت و" + str;
                    }
                }
                else if (Ragham > 69 && Ragham < 80)
                {
                    if (Ragham == 70)
                    {
                        return "هفتاد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "هفتادو" + str;
                    }
                }
                else if (Ragham > 79 && Ragham < 90)
                {
                    if (Ragham == 80)
                    {
                        return "هشتاد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "هشتادو" + str;
                    }
                }
                else if (Ragham > 89 && Ragham < 100)
                {
                    if (Ragham == 90)
                    {
                        return "نود";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "نودو" + str;
                    }
                }
                return "";
            }
            catch
            {
                return "-1";
            }
            finally
            {
            }

        }
        /// <summary>
        /// اعداد از 0   تا نود نه را به حروف تبدیل میکند
        /// </summary>
        /// <param name="Ragham">عدد بین 0 تا 99</param>
        /// <returns>حروف را برمی گرداند در صورت خطا -1  و درصورت عدد نامعتبر تهی برمی گرداند</returns>
        private static string ConvertRaghamToHorof0to99(int Ragham)
        {
            try
            {
                if (Ragham > 99) return "";
                if (Ragham < 20)
                {
                    return ConvertRaghamToHorof0to19(Ragham);
                }
                else if (Ragham < 100)
                {
                    return ConvertRaghamToHorof20to99(Ragham);
                }
                return "";
            }
            catch
            {
                return "";
            }
            finally
            { }
        }
        /// <summary>
        /// اعداد از صد  تانهصدو نودو نه را به حروف تبدیل میکند
        /// </summary>
        /// <param name="Ragham">عدد بین 100 تا 999</param>
        /// <returns>حروف را برمی گرداند در صورت خطا -1  و درصورت عدد نامعتبر تهی برمی گرداند</returns>
        private static string ConvertRaghamToHorof100to999(int Ragham)
        {
            try
            {
                //اعداد 100  تا 999
                if (!(Ragham > 99 && Ragham < 1000)) return "";
                // برای تبدیل به حروف  رقم دوم

                string str = ConvertRaghamToHorof0to99(int.Parse(Ragham.ToString().Substring(1)));
                if (Ragham > 99 && Ragham < 200)
                {
                    if (Ragham == 100)
                    {
                        return "صد";
                    }
                    else
                    {
                        return "صدو" + str;
                    }

                }
                else if (Ragham > 199 && Ragham < 300)
                {
                    if (Ragham == 200)
                    {
                        return "دویست";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "دویست و" + str;
                    }
                }
                else if (Ragham > 299 && Ragham < 400)
                {
                    if (Ragham == 300)
                    {
                        return "سیصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "سیصدو" + str;
                    }
                }
                else if (Ragham > 399 && Ragham < 500)
                {
                    if (Ragham == 400)
                    {
                        return "چهارصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "چهارصدو" + str;
                    }
                }
                else if (Ragham > 499 && Ragham < 600)
                {
                    if (Ragham == 500)
                    {
                        return "پانصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "پانصدو" + str;
                    }
                }
                else if (Ragham > 599 && Ragham < 700)
                {
                    if (Ragham == 600)
                    {
                        return "ششصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "ششصدو" + str;
                    }
                }
                else if (Ragham > 699 && Ragham < 800)
                {
                    if (Ragham == 700)
                    {
                        return "هفتصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "هفتصدو" + str;
                    }
                }
                else if (Ragham > 799 && Ragham < 900)
                {
                    if (Ragham == 800)
                    {
                        return "هشتصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "هشتصدو" + str;
                    }
                }
                else if (Ragham > 899 && Ragham < 1000)
                {
                    if (Ragham == 900)
                    {
                        return "نهصد";
                    }
                    else
                    {
                        //string str = ConvertRaghamToHorof0to20(Ragham.ToString().Substring(1));
                        return "نهصدو" + str;
                    }
                }
                return "";
            }
            catch
            {
                return "-1";
            }
            finally
            {
            }

        }
        /// <summary>
        /// اعداد از صفر تا 999 را به حروف تبدیل می کند
        /// </summary>
        /// <param name="Ragham"></param>
        /// <returns></returns>
        private static string ConvertRaghamToHorof0to999(int Ragham)
        {
            try
            {

                if (!(Ragham < 1000)) return "";

                if (Ragham < 20)
                {
                    return ConvertRaghamToHorof0to19(Ragham);
                }
                else if (Ragham > 19 && Ragham < 100)
                {
                    return ConvertRaghamToHorof20to99(Ragham);
                }
                else if (Ragham > 99 && Ragham < 1000)
                {
                    return ConvertRaghamToHorof100to999(Ragham);
                }
                else
                {
                    return "خطا برنامه نویسی";
                }
            }
            catch
            {
                return "-1";
            }
            finally
            {
            }

        }
        /// <summary>
        /// یک عدد سه رقمی و موقعیت انرا دریافت و به حروف تبدیل می کند
        /// </summary>
        /// <param name="Ragham">عدد سه رقمی</param>
        /// <param name="LocationRagham"> موقیت عدد 2 =هزار 3== میلیون 4= میلیارد</param>
        /// <returns></returns>
        private static string pishConvertRaghamToHorof3(int Ragham, int LocationRagham)
        {
            try
            {   //اگر عدد صفر نیازبه پیش ندارد
                if (Ragham == 0) return "";
                if (Ragham > 1000) return "";

                //string str = ConvertRaghamToHorof0to999(Ragham);
                if (Ragham == 0) return "";
                switch (LocationRagham)
                {
                    case 1:
                        return ConvertRaghamToHorof0to999(Ragham);
                    case 2:
                        return ConvertRaghamToHorof0to999(Ragham) + " هزار";
                    case 3:
                        return ConvertRaghamToHorof0to999(Ragham) + " میلیون ";
                    case 4:
                        return ConvertRaghamToHorof0to999(Ragham) + " میلیارد";
                    case 5:
                        return ConvertRaghamToHorof0to999(Ragham) + "تریلیارد";
                    case 6:

                        return ConvertRaghamToHorof0to999(Ragham) + " ??? ";
                    case 7:

                        return ConvertRaghamToHorof0to999(Ragham) + " ??? ";
                    case 8:

                        return ConvertRaghamToHorof0to999(Ragham) + " ??? ";
                    case 9:

                        return ConvertRaghamToHorof0to999(Ragham) + " ??? ";
                }

                return "";
            }
            catch
            {
                return "صفر";
            }
            finally
            {
            }

        }

        /// <summary>
        /// یک رقم را سه رقم سه رقم جدا می کند
        /// </summary>
        /// <param name="Ragham">رقم که باید حداکثر دوازده رقم باشد</param>
        /// <returns>جداشده سه رقم سه رقم را برمی گرداند</returns>
        public static string ConvertRaghamToJodaJoda(Int64 Ragham)
        {

            try
            {
                string Add = Ragham.ToString();
                Add = Add.Trim(); //فقط دوازده رقم را پشتیبانی می کند
                if (Add.Length > 15) Add = Add.Substring(0, 15);
                if (Add.Trim() == "")
                {

                    return "";
                }

                if ((Add.Length > 3 && Add.Length < 6) || Add.Length == 6)
                {
                    //RadMessageBox.Show(i.Substring(3, 1));
                    Add = Add.Substring(0, Add.Length - 3) + "," + Add.Substring(Add.Length - 3, 3);
                }
                else if ((Add.Length > 6 && Add.Length < 9) || Add.Length == 9)
                {

                    Add = Add.Substring(0, Add.Length - 6) +
                           "," + Add.Substring(Add.Length - 6, 3) +
                           "," + Add.Substring(Add.Length - 3, 3);

                }
                else if ((Add.Length > 9 && Add.Length < 12) || Add.Length == 12)
                {
                    //RadMessageBox.Show(i.Substring(3, 1));
                    Add = Add.Substring(0, Add.Length - 9) +
                          "," + Add.Substring(Add.Length - 9, 3) +
                          "," + Add.Substring(Add.Length - 6, 3) +
                    "," + Add.Substring(Add.Length - 3, 3);
                    //Add = Add.Substring(0, 3) +
                    //       "," + Add.Substring(3, 3) +
                    //       "," + Add.Substring(6, 3) +
                    //       "," + Add.Substring(9, Add.Length - 9);
                }
                else if (Add.Length > 12 && Add.Length < 15)
                {
                    //RadMessageBox.Show(i.Substring(3, 1));
                    Add = Add.Substring(0, 3) +
                           "," + Add.Substring(3, 3) +
                           "," + Add.Substring(6, 3) +
                            "," + Add.Substring(9, 3) +
                           "," + Add.Substring(12, Add.Length - 12);
                }
                else if (Add.Length < 3)
                {

                }

                return Add;
            }


            catch
            {
                //ClsGlobalClass.ShowMessageErrorFarsi(e, "ConvertRaghamToJodaJoda(Int64 Ragham)");
                return "";

            }
            finally
            {
                //ObjBankPatern.SqlConnection.Close();
                //cmd.Dispose();
                //dap.Dispose();
            }
        }
        #endregion ConvertRaghamToHorof

        //private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        //private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        //private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        //private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        //private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };
        //private static string getnum3(int num3)
        //{
        //    string s = "";
        //    int d3, d12;
        //    d12 = num3 % 100;
        //    d3 = num3 / 100;
        //    if (d3 != 0)
        //        s = sadgan[d3] + " و ";
        //    if ((d12 >= 10) && (d12 <= 19))
        //    {
        //        s = s + dahyek[d12 - 10];
        //    }
        //    else
        //    {
        //        int d2 = d12 / 10;
        //        if (d2 != 0)
        //            s = s + dahgan[d2] + " و ";
        //        int d1 = d12 % 10;
        //        if (d1 != 0)
        //            s = s + yakan[d1] + " و ";
        //        s = s.Substring(0, s.Length - 3);
        //    };
        //    return s;
        //}
        //public string num2str(string snum)
        //{
        //    string stotal = "";
        //    if (snum == "") return "صفر";
        //    if (snum == "0")
        //    {
        //        return yakan[0];
        //    }
        //    else
        //    {
        //        snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
        //        int L = snum.Length / 3 - 1;
        //        for (int i = 0; i <= L; i++)
        //        {
        //            int b = int.Parse(snum.Substring(i * 3, 3));
        //            if (b != 0)
        //                stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
        //        }
        //        stotal = stotal.Substring(0, stotal.Length - 3);
        //    }
        //    return stotal;
        //}
































        //public static string GET_Number_To_PersianString(string TXT)
        //{
        //    string RET = " ", STRVA = " ";
        //    string[] MainStr = STR_To_Int(TXT);
        //    int Q = 0;
        //    for (int i = MainStr.Length - 1; i >= 0; i--)
        //    {
        //        STRVA = " ";
        //        if (RET != " " && RET != null)
        //            STRVA = " و ";
        //        RET = Convert_STR(GETCountStr(MainStr[i]), Q) + STRVA + RET;
        //        Q++;
        //    }
        //    if (RET == " " || RET == null || RET == "  ")
        //        RET = "صفر";
        //    return RET;
        //}

        //private static string[] STR_To_Int(string STR)
        //{
        //    STR = GETCountStr(STR);
        //    string[] RET = new string[STR.Length / 3];
        //    int Q = 0;
        //    for (int I = 0; I < STR.Length; I += 3)
        //    {
        //        RET[Q] = STR.Substring(I, 3);
        //        Q++;
        //    }
        //    return RET;
        //}

        //private static string GETCountStr(string STR)
        //{
        //    string RET = STR;
        //    int LEN = (STR.Length / 3 + 1) * 3 - STR.Length;
        //    if (LEN < 3)
        //    {
        //        for (int i = 0; i < LEN; i++)
        //        {
        //            RET = "0" + RET;
        //        }
        //    }
        //    if (RET == "")
        //        return "000";
        //    return RET;
        //}

        //private static string Convert_STR(string INT, int Count)
        //{
        //    string RET = "";
        //    //یک صد
        //    if (Count == 0)
        //    {
        //        if (INT.Substring(1, 1) == "1" && INT.Substring(2, 1) != "0")
        //        {
        //            RET = GET_Number(3, Convert.ToInt32(INT.Substring(0, 1)), " ") + GET_Number(1, Convert.ToInt32(INT.Substring(2, 1)), "");
        //        }
        //        else
        //        {
        //            string STR = GET_Number(0, Convert.ToInt32(INT.Substring(2, 1)), "");
        //            RET = GET_Number(3, Convert.ToInt32(INT.Substring(0, 1)), GET_Number(2, Convert.ToInt32(INT.Substring(1, 1)), "") + STR) + GET_Number(2, Convert.ToInt32(INT.Substring(1, 1)), STR) + GET_Number(0, Convert.ToInt32(INT.Substring(2, 1)), "");
        //        }
        //    }
        //    //هزار
        //    else if (Count == 1)
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += " هزار";
        //    }
        //    //میلیون
        //    else if (Count == 2)
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += " میلیون";
        //    }
        //    //میلیارد
        //    else if (Count == 3)
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += " میلیارد";
        //    }
        //    //میلیارد
        //    else if (Count == 4)
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += " تیلیارد";
        //    }
        //    //میلیارد
        //    else if (Count == 5)
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += " بیلیارد";
        //    }
        //    else
        //    {
        //        RET = Convert_STR(INT, 0);
        //        RET += Count.ToString();
        //    }
        //    return RET;
        //}

        //private static string GET_Number(int Count, int Number, string VA)
        //{
        //    string RET = "";

        //    if (VA != "" && VA != null)
        //    {
        //        VA = " و ";
        //    }
        //    if (Count == 0 || Count == 1)
        //    {
        //        bool IsDah = Convert.ToBoolean(Count);
        //        string[] MySTR = new string[10];
        //        MySTR[1] = IsDah ? "یازده" : "یک" + VA;
        //        MySTR[2] = IsDah ? "دوازده" : "دو" + VA;
        //        MySTR[3] = IsDah ? "سیزده" : "سه" + VA;
        //        MySTR[4] = IsDah ? "چهارده" : "چهار" + VA;
        //        MySTR[5] = IsDah ? "پانزده" : "پنج" + VA;
        //        MySTR[6] = IsDah ? "شانزده" : "شش" + VA;
        //        MySTR[7] = IsDah ? "هفده" : "هفت" + VA;
        //        MySTR[8] = IsDah ? "هجده" : "هشت" + VA;
        //        MySTR[9] = IsDah ? "نوزده" : "نه" + VA;
        //        return MySTR[Number];
        //    }
        //    else if (Count == 2)
        //    {
        //        string[] MySTR = new string[10];
        //        MySTR[1] = "ده";
        //        MySTR[2] = "بیست" + VA;
        //        MySTR[3] = "سی" + VA;
        //        MySTR[4] = "چهل" + VA;
        //        MySTR[5] = "پنجاه" + VA;
        //        MySTR[6] = "شصت" + VA;
        //        MySTR[7] = "هفتاد" + VA;
        //        MySTR[8] = "هشتاد" + VA;
        //        MySTR[9] = "نود" + VA;
        //        return MySTR[Number];
        //    }
        //    else if (Count == 3)
        //    {
        //        string[] MySTR = new string[10];
        //        MySTR[1] = "یکصد" + VA;
        //        MySTR[2] = "دویست" + VA;
        //        MySTR[3] = "سیصد" + VA;
        //        MySTR[4] = "چهارصد" + VA;
        //        MySTR[5] = "پانصد" + VA;
        //        MySTR[6] = "ششصد" + VA;
        //        MySTR[7] = "هفتصد" + VA;
        //        MySTR[8] = "هشتصد" + VA;
        //        MySTR[9] = "نهصد" + VA;
        //        return MySTR[Number];
        //    }
        //    return RET;
        //}
    }
}
