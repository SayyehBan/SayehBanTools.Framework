using System.Linq;

namespace SayehBanTools.Framework.GenerateValue
{
    public static class LegalNationalID
    {
        public static bool IsValidLegalNationalID(string nationalId)
        {
            // شناسه ملی حقوقی حتماً باید ۱۱ رقم و فقط شامل اعداد باشد
            if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 11 || !nationalId.All(char.IsDigit))
                return false;

            // اگر همه ارقام صفر باشند نامعتبر است
            if (nationalId.All(c => c == '0'))
                return false;

            int tens = nationalId[9] - '0'; // رقم دهم
            int controlDigit = nationalId[10] - '0'; // رقم یازدهم (کنترلی)
            int coefficient = tens + 2; // ضریب پایه
            int[] weights = { 29, 27, 23, 19, 17, 29, 27, 23, 19, 17 }; // ضرایب استاندارد الگوریتم

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += ((nationalId[i] - '0') + coefficient) * weights[i];
            }

            int remainder = sum % 11;
            if (remainder == 10)
            {
                remainder = 0;
            }

            return remainder == controlDigit;
        }
    }
}
