using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SayehBanTools.Framework.Encryptor;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Model;
using System;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.Connection
{
    /// <summary>
    /// کلاس مدیریت اتصال به RabbitMQ با تنظیمات رمزنگاری‌شده
    /// </summary>
    public partial class RabbitMQConnection : IDisposable
    {
        private readonly RabbitMqConnectionSettings _rabbitMqConnectionSettings;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private IConnection _connection;
        private IChannel _channel;
        private bool _disposed;

        /// <summary>
        /// پراپرتی اتصال به RabbitMQ
        /// </summary>
        public IConnection Connection
        {
            get => _connection;
            set => _connection = value;
        }

        /// <summary>
        /// پراپرتی کانال RabbitMQ
        /// </summary>
        public IChannel Channel
        {
            get => _channel;
            set => _channel = value;
        }

        /// <summary>
        /// سازنده پیش‌فرض
        /// </summary>
        public RabbitMQConnection()
        {
        }

        /// <summary>
        /// سازنده با تنظیمات RabbitMQ
        /// </summary>
        /// <param name="rabbitMqConnectionSettings">تنظیمات اتصال</param>
        public RabbitMQConnection(IOptions<RabbitMqConnectionSettings> rabbitMqConnectionSettings)
        {
            _rabbitMqConnectionSettings = rabbitMqConnectionSettings?.Value ?? throw new ArgumentNullException(nameof(rabbitMqConnectionSettings));
            _hostname = DecryptSetting(_rabbitMqConnectionSettings.Hostname);
            _username = DecryptSetting(_rabbitMqConnectionSettings.Username);
            _password = DecryptSetting(_rabbitMqConnectionSettings.Password);
            _port = int.TryParse(DecryptSetting(_rabbitMqConnectionSettings.Port), out int port) ? port : throw new InvalidOperationException("Invalid port number.");
        }

        private string DecryptSetting(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                throw new ArgumentNullException(nameof(encryptedValue));
            if (string.IsNullOrEmpty(_rabbitMqConnectionSettings?.InitVector))
                throw new ArgumentNullException(nameof(_rabbitMqConnectionSettings.InitVector));
            if (string.IsNullOrEmpty(_rabbitMqConnectionSettings?.PassPhrase))
                throw new ArgumentNullException(nameof(_rabbitMqConnectionSettings.PassPhrase));

            return StringEncryptor.DecryptAsync(encryptedValue, _rabbitMqConnectionSettings.InitVector, _rabbitMqConnectionSettings.PassPhrase)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// ایجاد اتصال به RabbitMQ به‌صورت async
        /// </summary>
        public async Task CreateRabbitMQConnectionAsync()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname ?? throw new InvalidOperationException("Hostname is not initialized."),
                    UserName = _username ?? throw new InvalidOperationException("Username is not initialized."),
                    Password = _password ?? throw new InvalidOperationException("Password is not initialized."),
                    Port = _port,
                    AutomaticRecoveryEnabled = true
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot create connection: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// بررسی اتصال به RabbitMQ به‌صورت async
        /// </summary>
        /// <returns>وضعیت اتصال</returns>
        public async Task<bool> CheckRabbitMQConnectionAsync()
        {
            if (_connection?.IsOpen == true)
            {
                return true;
            }

            await CreateRabbitMQConnectionAsync();
            return _connection?.IsOpen == true;
        }

        /// <summary>
        /// آزادسازی منابع
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing RabbitMQ connection: {ex.Message}");
            }

            _disposed = true;
        }
    }
}