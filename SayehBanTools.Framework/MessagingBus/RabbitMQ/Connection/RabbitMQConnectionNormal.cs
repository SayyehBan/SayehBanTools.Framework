using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Model;
using System;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.Connection
{
    /// <summary>
    /// کلاس مدیریت اتصال به RabbitMQ بدون رمزنگاری
    /// </summary>
    public class RabbitMQConnectionNormal : IDisposable
    {
        private readonly RabbitMqConnectionSettingsNormal _rabbitMqConnectionSettingsNormal;
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
        public RabbitMQConnectionNormal()
        {
        }

        /// <summary>
        /// سازنده با تنظیمات RabbitMQ
        /// </summary>
        /// <param name="rabbitMqConnectionSettingsNormal">تنظیمات اتصال</param>
        public RabbitMQConnectionNormal(IOptions<RabbitMqConnectionSettingsNormal> rabbitMqConnectionSettingsNormal)
        {
            _rabbitMqConnectionSettingsNormal = rabbitMqConnectionSettingsNormal?.Value ?? throw new ArgumentNullException(nameof(rabbitMqConnectionSettingsNormal));
            _hostname = _rabbitMqConnectionSettingsNormal.Hostname ?? throw new ArgumentNullException(nameof(_rabbitMqConnectionSettingsNormal.Hostname));
            _username = _rabbitMqConnectionSettingsNormal.Username ?? throw new ArgumentNullException(nameof(_rabbitMqConnectionSettingsNormal.Username));
            _password = _rabbitMqConnectionSettingsNormal.Password ?? throw new ArgumentNullException(nameof(_rabbitMqConnectionSettingsNormal.Password));
            _port = int.TryParse(_rabbitMqConnectionSettingsNormal.Port, out int port) ? port : throw new InvalidOperationException("Invalid port number.");
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