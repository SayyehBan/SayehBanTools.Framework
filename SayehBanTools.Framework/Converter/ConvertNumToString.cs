using System;
using System.Collections.Generic;

namespace SayehBanTools.Framework.Converter
{
    /// <summary>
    /// تبدیل عدد به حروف
    /// </summary>
    public class ConvertNumToString
    {
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string convert(string number)
            {
                int length = number.Length;
                int level = 0;
                string str = "";
                List<int> list = new List<int>();
                bool flag = false;
                if (length == 0)
                {
                    return "لطفا عدد مورد نظر را وارد کنید !";
                }
                try
                {
                    while (length != 0)
                    {
                        if (length > 3)
                        {
                            list.Add(Convert.ToInt32(number.Substring(length - 3, 3)));
                            length -= 3;
                        }
                        else
                        {
                            list.Add(Convert.ToInt32(number.Substring(0, length)));
                            length = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    return "لطفا فقط عدد وارد کنید !";
                }
                foreach (int num3 in list)
                {
                    if (num3 != 0)
                    {
                        if (Leveler(level, flag) == "صفر")
                        {
                            break;
                        }
                        str = Leveler(level, flag) + str;
                        str = Thousand(num3) + str;
                        flag = true;
                    }
                    level++;
                }
                if (str == "")
                {
                    return "صفر";
                }
                return str;
            }
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="Literal"></param>
        /// <param name="digit_sadgan"></param>
        /// <param name="haveup"></param>
        /// <returns></returns>

        private static string Hundreds(string Literal, int digit_sadgan, bool haveup)
            {
                if (haveup)
                {
                    Literal = Literal + " و ";
                }
                switch (digit_sadgan)
                {
                    case 1:
                        Literal = Literal + "یکصد";
                        return Literal;

                    case 2:
                        Literal = Literal + "دویست";
                        return Literal;

                    case 3:
                        Literal = Literal + "سیصد";
                        return Literal;

                    case 4:
                        Literal = Literal + "چهارصد";
                        return Literal;

                    case 5:
                        Literal = Literal + "پانصد";
                        return Literal;

                    case 6:
                        Literal = Literal + "ششصد";
                        return Literal;

                    case 7:
                        Literal = Literal + "هفتصد";
                        return Literal;

                    case 8:
                        Literal = Literal + "هشتصد";
                        return Literal;

                    case 9:
                        Literal = Literal + "نهصد";
                        return Literal;
                }
                return Literal;
            }
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="level"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private static string Leveler(int level, bool flag)
            {
                if (flag)
                {
                    switch (level)
                    {
                        case 0:
                            return " ";

                        case 1:
                            return " هزار و ";

                        case 2:
                            return " میلیون و ";

                        case 3:
                            return " میلیارد و ";

                        case 4:
                            return " بیلیون و ";

                        case 5:
                            return " بیلیارد و ";

                        case 6:
                            return " تریلیون و ";

                        case 7:
                            return " تریلیارد و ";

                        case 8:
                            return " کادریلیون و ";
                    }
                    return "صفر";
                }
                switch (level)
                {
                    case 0:
                        return " ";

                    case 1:
                        return " هزار ";

                    case 2:
                        return " میلیون ";

                    case 3:
                        return " میلیارد ";

                    case 4:
                        return " بیلیون ";

                    case 5:
                        return " بیلیارد ";

                    case 6:
                        return " تریلیون ";

                    case 7:
                        return " تریلیارد ";

                    case 8:
                        return " کادریلیون ";
                }
                return "صفر";
            }
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="Literal"></param>
        /// <param name="digit_yekan"></param>
        /// <param name="digit_dahgan"></param>
        /// <param name="haveup"></param>
        /// <returns></returns>
        private static string Ones(string Literal, int digit_yekan, int digit_dahgan, bool haveup)
            {
                if (digit_dahgan != 1)
                {
                    if (!((digit_yekan != 0) || haveup))
                    {
                        return (Literal = "صفر");
                    }
                    if ((digit_yekan == 0) && haveup)
                    {
                        return Literal;
                    }
                    if (haveup)
                    {
                        Literal = Literal + " و ";
                    }
                    switch (digit_yekan)
                    {
                        case 1:
                            Literal = Literal + "یک";
                            return Literal;

                        case 2:
                            Literal = Literal + "دو";
                            return Literal;

                        case 3:
                            Literal = Literal + "سه";
                            return Literal;

                        case 4:
                            Literal = Literal + "چهار";
                            return Literal;

                        case 5:
                            Literal = Literal + "پنج";
                            return Literal;

                        case 6:
                            Literal = Literal + "شش";
                            return Literal;

                        case 7:
                            Literal = Literal + "هفت";
                            return Literal;

                        case 8:
                            Literal = Literal + "هشت";
                            return Literal;
                    }
                    Literal = Literal + "نه";
                }
                return Literal;
            }
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="Literal"></param>
        /// <param name="digit_yekan"></param>
        /// <param name="digit_dahgan"></param>
        /// <param name="haveup"></param>
        /// <returns></returns>
        private static string Tens(string Literal, int digit_yekan, int digit_dahgan, bool haveup)
            {
                if (haveup && (digit_dahgan != 0))
                {
                    Literal = Literal + " و ";
                }
                switch (digit_dahgan)
                {
                    case 2:
                        Literal = Literal + "بیست";
                        return Literal;

                    case 3:
                        Literal = Literal + "سی";
                        return Literal;

                    case 4:
                        Literal = Literal + "چهل";
                        return Literal;

                    case 5:
                        Literal = Literal + "پنجاه";
                        return Literal;

                    case 6:
                        Literal = Literal + "شصت";
                        return Literal;

                    case 7:
                        Literal = Literal + "هفتاد";
                        return Literal;

                    case 8:
                        Literal = Literal + "هشتاد";
                        return Literal;

                    case 9:
                        Literal = Literal + "نود";
                        return Literal;

                    case 1:
                        switch (digit_yekan)
                        {
                            case 0:
                                Literal = Literal + "ده";
                                return Literal;

                            case 1:
                                Literal = Literal + "یازده";
                                return Literal;

                            case 2:
                                Literal = Literal + "دوازده";
                                return Literal;

                            case 3:
                                Literal = Literal + "سیزده";
                                return Literal;

                            case 4:
                                Literal = Literal + "چهارده";
                                return Literal;

                            case 5:
                                Literal = Literal + "پانزده";
                                return Literal;

                            case 6:
                                Literal = Literal + "شانزده";
                                return Literal;

                            case 7:
                                Literal = Literal + "هفده";
                                return Literal;

                            case 8:
                                Literal = Literal + "هجده";
                                return Literal;

                            case 9:
                                Literal = Literal + "نوزده";
                                return Literal;
                        }
                        return Literal;
                }
                return Literal;
            }
        /// <summary>
        /// تبدیل عدد به حروف
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Thousand(int input)
            {
                List<int> list = new List<int>();
                while (input >= 0)
                {
                    if ((input == 0) && (list.Count == 0))
                    {
                        list.Add(input);
                        break;
                    }
                    if ((input == 0) && (list.Count != 0))
                    {
                        break;
                    }
                    list.Add(input % 10);
                    input /= 10;
                }
                if (list.Count == 1)
                {
                    return Ones("", list[0], 0, false);
                }
                if (list.Count == 2)
                {
                    return Ones(Tens("", list[0], list[1], false), list[0], list[1], true);
                }
                return Ones(Tens(Hundreds("", list[2], false), list[0], list[1], true), list[0], list[1], true);
            }

            //طریقه صدا کردن دستور
            //this.textBox2.Text = ClsConvertNumToString.convert(this.textBox1.Text);
        }
}
