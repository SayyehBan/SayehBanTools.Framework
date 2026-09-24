using Newtonsoft.Json;
using RabbitMQ.Client;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Connection;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Model;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.SendMessage
{
    /// <summary>
    /// کلاس ارسال پیام به RabbitMQ با تنظیمات رمزنگاری‌شده
    /// </summary>
    public class RabbitMQMessageBus : ISendMessages
    {
        private readonly RabbitMQConnection _rabbitMqConnection;

        /// <summary>
        /// سازنده برای تزریق وابستگی
        /// </summary>
        /// <param name="rabbitMqConnection">اتصال به RabbitMQ</param>
        public RabbitMQMessageBus(RabbitMQConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));
        }

        /// <summary>
        /// ارسال پیام به RabbitMQ به‌صورت async
        /// </summary>
        /// <param name="message">پیام</param>
        /// <param name="exchange">نام اکسچنج</param>
        /// <param name="queueName">نام صف</param>
        public async Task SendMessageAsync(BaseMessage message, string exchange, string queueName)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!await _rabbitMqConnection.CheckRabbitMQConnectionAsync())
                throw new InvalidOperationException("Cannot connect to RabbitMQ.");

            var channel = _rabbitMqConnection.Channel;
            if (channel == null || channel.IsClosed)
                throw new InvalidOperationException("RabbitMQ channel is not initialized or closed.");

            if (queueName != null)
            {
                await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            if (exchange != null)
            {
                await channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
            }

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
            exchange: exchange ?? "",
            routingKey: queueName ?? "",
            mandatory: false,
            basicProperties: properties,
            body: body); // تبدیل byte[] به ReadOnlyMemory<byte>
        }
    }
}
