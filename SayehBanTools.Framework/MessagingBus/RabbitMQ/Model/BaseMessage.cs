using System;

namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.Model
{
    /// <summary>
    /// این کلاس مدل پغام RabbitMQ
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// شناسه پیام
        /// </summary>
        public Guid MessageId { get; set; } = Guid.NewGuid();
        /// <summary>
        /// زمان ایجاد پیغام
        /// </summary>
        public DateTime Creationtime { get; set; } = DateTime.UtcNow;
    }
}
