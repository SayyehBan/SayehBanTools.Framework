using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace SayehBanTools.Framework.Password
{
    /// <summary>  
    /// این کلاس برای مدیریت کلمات پسورد های ضعیف استفاده میشود  
    /// </summary>  
    public class WorstPasswords
    {
        /// <summary>  
        /// این متد برای محاسبه هش استفاده میشود  
        /// </summary>  
        public List<string> CommonPassword { get; set; }
        /// <summary>  
        /// این متد برای محاسبه هش استفاده میشود  
        /// </summary>  
        public WorstPasswords()
        {
            CommonPassword = new List<string>();
        }
        /// <summary>  
        /// این متد برای محاسبه هش استفاده میشود  
        /// </summary>  
        /// <param name="directFile"></param>  
        /// <returns></returns>  
        public async Task LoadCommonPasswords(string directFile)
        {
            if (CommonPassword.Count == 0)
            {
                using (StreamReader reader = new StreamReader(directFile))
                {
                    while (await reader.ReadLineAsync() is string line)
                    {
                        string hashedLine = ComputeSha256Hash(line);
                        CommonPassword.Add(hashedLine);
                    }
                }
            }
        }
        /// <summary>  
        /// این متد برای محاسبه هش استفاده میشود  
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashedBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }

    /*
     طریقه استفاده از دستورمورد نظر
    public class MyPasswordValidator : IPasswordValidator<User>
    {
        private readonly IWebHostEnvironment _environment;
        private WorstPasswords worstPasswords;

        public MyPasswordValidator(IWebHostEnvironment environment)
        {
            _environment = environment;
            worstPasswords = new WorstPasswords();
        }


        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "file", "worst-passwords.txt");
            await worstPasswords.LoadCommonPasswords(filePath);

            string hashedPassword = worstPasswords.ComputeSha256Hash(password);

            if (worstPasswords.CommonPassword.Contains(hashedPassword))
            {
                return GetFailedIdentityResult();
            }

            return IdentityResult.Success;
        }

        private IdentityResult GetFailedIdentityResult()
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "CommonPassword",
                Description = "پسورد شما قابل شناسایی توسط ربات های هکر است! لطفا یک پسورد قوی انتخاب کنید",
            });
        }
    }
     */
}