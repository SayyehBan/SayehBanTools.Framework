using SayehBanTools.Framework.Encryptor;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace SayehBanTools.Framework.ConnectionDB
{
    /// <summary>
    /// این کلاس برای اتصال به دیتابیس اسکیوال سرور استفاده میشود
    /// </summary>
    public class SqlServerConnection
    {
        /// <summary>
        /// این متد برای ساخت اتصال به دیتابیس اسکیوال سرور استفاده میشود به صورت کدگذاری
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <param name="initVector"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static async Task<string> ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password, string initVector, string passPhrase)
        {
            try
            {
                if (string.IsNullOrEmpty(DataSource) || string.IsNullOrEmpty(InitialCatalog) ||
                string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(initVector) || string.IsNullOrEmpty(passPhrase))
                {
                    throw new ArgumentNullException("All parameters must be provided.");
                }
                string dataSource = await StringEncryptor.DecryptAsync(DataSource, initVector, passPhrase);
                string initialCatalog = await StringEncryptor.DecryptAsync(InitialCatalog, initVector, passPhrase);
                string userId = await StringEncryptor.DecryptAsync(UserId, initVector, passPhrase);
                string password = await StringEncryptor.DecryptAsync(Password, initVector, passPhrase);
                string connectionString = $"Data Source={dataSource};" +
                                         $"Initial Catalog={initialCatalog};" +
                                         $"User ID={userId};" +
                                         $"Password={password};" +
                                         "Connect Timeout=0;" +
                                         "Max Pool Size=20000;" +
                                         "Integrated Security=False;" +
                                         "Trust Server Certificate=True;";

                return connectionString;
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
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// این متد برای ساخت اتصال به دیتابیس اسکیوال سرور استفاده میشود معمولی
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password)
        {
            string connectionString = $"Data Source={DataSource};" +
                                     $"Initial Catalog={InitialCatalog};" +
                                     $"User ID={UserId};" +
                                     $"Password={Password};" +
                                     "Connect Timeout=0;" +
                                     "Max Pool Size=20000;" +
                                     "Integrated Security=False;" +
                                     "Trust Server Certificate=True;";

            return connectionString;
        }
        /// <summary>
        /// این متد برای ساخت اتصال به دیتابیس اسکیوال سرور استفاده میشود به صورت کدگذاری
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <param name="ConnectTimeout"></param>
        /// <param name="MaxPoolSize"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <param name="initVector"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static async Task<string> ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password, short ConnectTimeout, int MaxPoolSize, bool IntegratedSecurity, string initVector, string passPhrase)
        {
            try
            {
                if (string.IsNullOrEmpty(DataSource) || string.IsNullOrEmpty(InitialCatalog) ||
                string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(initVector) || string.IsNullOrEmpty(passPhrase))
                {
                    throw new ArgumentNullException("All parameters must be provided.");
                }
                string dataSource = await StringEncryptor.DecryptAsync(DataSource, initVector, passPhrase);
                string initialCatalog = await StringEncryptor.DecryptAsync(InitialCatalog, initVector, passPhrase);
                string userId = await StringEncryptor.DecryptAsync(UserId, initVector, passPhrase);
                string password = await StringEncryptor.DecryptAsync(Password, initVector, passPhrase);
                string connectionString = $"Data Source={dataSource};" +
                                         $"Initial Catalog={initialCatalog};" +
                                         $"User ID={userId};" +
                                         $"Password={password};" +
                                         $"Connect Timeout={ConnectTimeout};" +
                                         $"Max Pool Size={MaxPoolSize};" +
                                         $"Integrated Security={IntegratedSecurity};";

                return connectionString;
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
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// این متد برای ساخت اتصال به دیتابیس اسکیوال سرور استفاده میشود معمولی
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <param name="ConnectTimeout"></param>
        /// <param name="MaxPoolSize"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <returns></returns>
        public static string ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password, short ConnectTimeout, int MaxPoolSize, bool IntegratedSecurity)
        {
            string connectionString = $"Data Source={DataSource};" +
                                     $"Initial Catalog={InitialCatalog};" +
                                     $"User ID={UserId};" +
                                     $"Password={Password};" +
                                     $"Connect Timeout={ConnectTimeout};" +
                                     $"Max Pool Size={MaxPoolSize};" +
                                     $"Integrated Security={IntegratedSecurity};";

            return connectionString;
        }
        /// <summary>
        /// این متد برای ساخت اتصال به دیتابیس اسکیوال سرور استفاده میشود به صورت کدگذاری
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <param name="ConnectTimeout"></param>
        /// <param name="MaxPoolSize"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <param name="TrustServerCertificate"></param>
        /// <param name="initVector"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static async Task<string> ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password, short ConnectTimeout, int MaxPoolSize, bool IntegratedSecurity, bool TrustServerCertificate, string initVector, string passPhrase)
        {
            try
            {
                if (string.IsNullOrEmpty(DataSource) || string.IsNullOrEmpty(InitialCatalog) ||
                string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(initVector) || string.IsNullOrEmpty(passPhrase))
                {
                    throw new ArgumentNullException("All parameters must be provided.");
                }
                string dataSource = await StringEncryptor.DecryptAsync(DataSource, initVector, passPhrase);
                string initialCatalog = await StringEncryptor.DecryptAsync(InitialCatalog, initVector, passPhrase);
                string userId = await StringEncryptor.DecryptAsync(UserId, initVector, passPhrase);
                string password = await StringEncryptor.DecryptAsync(Password, initVector, passPhrase);
                string connectionString = $"Data Source={dataSource};" +
                                         $"Initial Catalog={initialCatalog};" +
                                         $"User ID={userId};" +
                                         $"Password={password};" +
                                         $"Connect Timeout={ConnectTimeout};" +
                                         $"Max Pool Size={MaxPoolSize};" +
                                         $"Integrated Security={IntegratedSecurity};" +
                                         $"Trust Server Certificate={TrustServerCertificate};";

                return connectionString;
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
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// رشته اتصال به صورت معمولی برای ارتباط با اسکیوال سرور
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        /// <param name="ConnectTimeout"></param>
        /// <param name="MaxPoolSize"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <param name="TrustServerCertificate"></param>
        /// <returns></returns>
        public static string ConnectionString(string DataSource, string InitialCatalog, string UserId, string Password, short ConnectTimeout, int MaxPoolSize, bool IntegratedSecurity, bool TrustServerCertificate)
        {
            string connectionString = $"Data Source={DataSource};" +
                                     $"Initial Catalog={InitialCatalog};" +
                                     $"User ID={UserId};" +
                                     $"Password={Password};" +
                                     $"Connect Timeout={ConnectTimeout};" +
                                     $"Max Pool Size={MaxPoolSize};" +
                                     $"Integrated Security={IntegratedSecurity};" +
                                     $"Trust Server Certificate={TrustServerCertificate};";

            return connectionString;
        }
    }
}
