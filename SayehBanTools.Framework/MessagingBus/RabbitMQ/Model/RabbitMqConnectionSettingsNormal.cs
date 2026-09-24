namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.Model
{
    /// <summary>
    /// تنظمیات معمولی rabbitmq برای اتصال
    /// </summary>
    public class RabbitMqConnectionSettingsNormal
    {
        /// <summary>
        /// نام هاست
        /// </summary>
        public string Hostname { get; set; }
        /// <summary>
        /// پورت
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// رمز عبور
        /// </summary>
        public string Password { get; set; }
    }


    /*
     طریقه دادن اطلاعات به 
    RabbitMQ in program.cs
    var rabbitMqConnectionSettings = new RabbitMqConnectionSettings();
    rabbitMqConnectionSettings.Hostname = "localhost";
    rabbitMqConnectionSettings.Port = "5672"; // Default RabbitMQ port
    rabbitMqConnectionSettings.Username = "guset";
    rabbitMqConnectionSettings.Password = "guest";
    builder.Services.Configure<RabbitMqConnectionSettingsNormal>((IConfiguration)rabbitMqConnectionSettingsNormal);
 
     */
}