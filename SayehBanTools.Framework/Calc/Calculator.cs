using System;

namespace SayehBanTools.Framework.Calc
{
    /// <summary>
    /// انجام عملیات ریاضی مانند ضرب جمع و ...
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// انجام عملیات ضرب
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static double Multiply(params double[] numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }
            double result = 1;
            foreach (double number in numbers)
            {
                result *= number;
            }
            return result;
        }

        /// <summary>
        /// انجام عملیات جمع
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static double Add(params double[] numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }
            double result = 0;
            foreach (double number in numbers)
            {
                result += number;
            }
            return result;
        }

        /// <summary>
        /// انجام عملیات منفی
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static double Subtract(params double[] numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }
            if (numbers.Length < 2)
            {
                throw new ArgumentException("Subtract requires at least two numbers.");
            }

            double result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result -= numbers[i];
            }
            return result;
        }

        /// <summary>
        /// انجام عملیات تقسیم
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DivideByZeroException"></exception>
        public static double Divide(params double[] numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }
            if (numbers.Length < 2)
            {
                throw new ArgumentException("At least two numbers are required for division.");
            }

            double result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] == 0)
                {
                    throw new DivideByZeroException("Division by zero is not allowed.");
                }
                result /= numbers[i];
            }
            return result;
        }

        /// <summary>
        /// محاسبه مبلغ درصد تخفیف
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal Discount(decimal Amount, short percent)
        {
            if (Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.", nameof(Amount));
            }
            if (percent < 0)
            {
                throw new ArgumentException("Percent cannot be negative.", nameof(percent));
            }
            decimal total = Math.Round(Amount - ((Amount * percent) / 100), 2);
            return total;
        }

        /// <summary>
        /// دریافت درصد تخفیف یا مالیات
        /// </summary>
        /// <param name="Amount">مبلغ تخفیف یا مالیات</param>
        /// <param name="Price">قیمت اصلی</param>
        /// <returns>درصد تخفیف یا مالیات</returns>
        public static decimal GetPercent(decimal Amount, decimal Price)
        {
            if (Price <= 0)
            {
                throw new ArgumentException("Price must be positive.", nameof(Price));
            }
            return (Amount / Price) * 100;
        }

        /// <summary>
        /// محاسبه مبلغ درصد مالیات
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal Taxation(decimal Amount, short percent)
        {
            if (Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.", nameof(Amount));
            }
            if (percent < 0)
            {
                throw new ArgumentException("Percent cannot be negative.", nameof(percent));
            }
            decimal total = Math.Round(Amount + ((Amount * percent) / 100), 2);
            return total;
        }
    }
}