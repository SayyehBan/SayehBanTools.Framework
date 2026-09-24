using SayehBanTools.Framework.Encryptor;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace SayehBanTools.Framework.ConnectionDB
{
    /// <summary>
    /// این کلاس برای سرویس های رابیت مق استفاده میشود
    /// </summary>
    public partial class RabbitMQConnection
    {
        /// <summary>
        /// آدرس پیش فرض RabbitMQ
        /// </summary>
        /// <returns></returns>
        public static Uri DefaultConnection()
        {
            string connection = "amqp://guest:guest@localhost:5672";
            Uri uri = new Uri(connection);
            return uri;
        }
        /// <summary>
        /// آدرس‌دهی به RabbitMQ با اطلاعات رمزگشایی‌شده
        /// </summary>
        /// <param name="encryptedUsername">نام کاربری رمزنگاری‌شده</param>
        /// <param name="encryptedPassword">رمز عبور رمزنگاری‌شده</param>
        /// <param name="encryptedUrl">آدرس سرور رمزنگاری‌شده</param>
        /// <param name="encryptedPort">پورت رمزنگاری‌شده</param>
        /// <param name="initVector">وکتور اولیه</param>
        /// <param name="passPhrase">عبارت عبور</param>
        /// <returns>Uri برای اتصال به RabbitMQ</returns>
        public static async Task<Uri> RabbitMQAsync(string encryptedUsername, string encryptedPassword, string encryptedUrl, string encryptedPort, string initVector, string passPhrase)
        {
            try
            {
                // اعتبارسنجی ورودی‌ها
                if (string.IsNullOrEmpty(encryptedUsername) || string.IsNullOrEmpty(encryptedPassword) ||
                    string.IsNullOrEmpty(encryptedUrl) || string.IsNullOrEmpty(encryptedPort) ||
                    string.IsNullOrEmpty(initVector) || string.IsNullOrEmpty(passPhrase))
                {
                    throw new ArgumentNullException("All parameters must be provided.");
                }

                // رمزگشایی اطلاعات
                string username = await StringEncryptor.DecryptAsync(encryptedUsername, initVector, passPhrase);
                string password = await StringEncryptor.DecryptAsync(encryptedPassword, initVector, passPhrase);
                string url = await StringEncryptor.DecryptAsync(encryptedUrl, initVector, passPhrase);
                string port = await StringEncryptor.DecryptAsync(encryptedPort, initVector, passPhrase);

                // ساخت رشته اتصال
                string connectionString = $"amqp://{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}@{url}:{port}";

                // ایجاد Uri
                return new Uri(connectionString);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Invalid Base64 string in encrypted data.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Decryption failed. Check the initVector or passPhrase.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create RabbitMQ URI.", ex);
            }
        }
        /// <summary>
        /// آدرس دهی به صورت معمولی
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Url"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public static Uri RabbitMQ(string Username, string Password, string Url, string Port)
        {
            string connection = $"amqp://{Username}:{Password}@{Url}:{Port}";
            Uri uri = new Uri(connection);
            return uri;
        }
        /// <summary>
        /// آدرس دهی به صورت کد گذاری و CloadAMQP
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Url"></param>
        /// <param name="InitVector"></param>
        /// <param name="PassPhrase"></param>
        /// <returns></returns>
        public static async Task<Uri> CloudAMQPAsync(string Username, string Password, string Url, string InitVector, string PassPhrase)
        {
            try
            {
                // اعتبارسنجی ورودی‌ها
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(InitVector) ||
                string.IsNullOrEmpty(PassPhrase)
                )
                {
                    throw new ArgumentNullException("All parameters must be provided.");
                }
                string username = await StringEncryptor.DecryptAsync(Username, InitVector, PassPhrase);
                string password = await StringEncryptor.DecryptAsync(Password, InitVector, PassPhrase);
                string url = await StringEncryptor.DecryptAsync(Url, InitVector, PassPhrase);

                string connection = $"amqps://{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}@{url}";
                // Uri uri = new Uri(connection);
                return new Uri(connection);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Invalid Base64 string in encrypted data.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Decryption failed. Check the initVector or passPhrase.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create RabbitMQ URI.", ex);
            }

        }
        /// <summary>
        /// آدرس دهی به صورت معمولی و CloadAMQP
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static Uri CloudAMQP(string Username, string Password, string Url)
        {

            string connection = $"amqps://{Username}:{Password}@{Url}";
            Uri uri = new Uri(connection);
            return uri;
        }
    }

    //string plainText = "متن مورد نظر برای رمزنگاری";
    //string encryptedText = StringEncryptor.Encrypt(plainText);
    //string encryptedText = "متن رمزنگاری شده";
    //string plainText = StringEncryptor.Decrypt(encryptedText);
    //string plainText = "ConnectionString||密文";
    //string connectionString = StringEncryptor.DecryptConnectionString(plainText);
}