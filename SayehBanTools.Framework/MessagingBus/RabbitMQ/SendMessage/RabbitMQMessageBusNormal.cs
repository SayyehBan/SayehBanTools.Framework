using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Connection;
using SayehBanTools.Framework.MessagingBus.RabbitMQ.Model;

namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.SendMessage
{
    /// <summary>
    /// کلاس ارسال پیام به RabbitMQ بدون رمزنگاری
    /// </summary>
    public class RabbitMQMessageBusNormal : ISendMessages
    {
        private readonly RabbitMQConnectionNormal _rabbitMqConnectionNormal;

        /// <summary>
        /// سازنده برای تزریق وابستگی
        /// </summary>
        /// <param name="rabbitMqConnectionNormal">اتصال به RabbitMQ</param>
        public RabbitMQMessageBusNormal(RabbitMQConnectionNormal rabbitMqConnectionNormal)
        {
            _rabbitMqConnectionNormal = rabbitMqConnectionNormal ?? throw new ArgumentNullException(nameof(rabbitMqConnectionNormal));
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

            if (!await _rabbitMqConnectionNormal.CheckRabbitMQConnectionAsync())
                throw new InvalidOperationException("Cannot connect to RabbitMQ.");

            var channel = _rabbitMqConnectionNormal.Channel;
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