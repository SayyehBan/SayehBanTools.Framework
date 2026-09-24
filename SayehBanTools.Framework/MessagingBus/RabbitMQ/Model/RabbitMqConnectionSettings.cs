namespace SayehBanTools.Framework.MessagingBus.RabbitMQ.Model
{
    /// <summary>
    /// این کلاس مدل تنظیمات اتصال به RabbitMQ
    /// </summary>
    public class RabbitMqConnectionSettings
    {
        /// <summary>
        /// نام میزبان
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
        /// <summary>
        /// مقدار اولیه
        /// </summary>
        public string InitVector { get; set; }
        /// <summary>
        /// رمز عبور
        /// </summary>
        public string PassPhrase { get; set; }
    }


    /*
     طریقه دادن اطلاعات به 
    RabbitMQ in program.cs
    var rabbitMqConnectionSettings = new RabbitMqConnectionSettings();
    rabbitMqConnectionSettings.Hostname = "HyyV3VbQHmNeQglHqlhcKQ==";
    rabbitMqConnectionSettings.Port = "EpPdu9pj1Nus2am5LbmM6w=="; // Default RabbitMQ port
    rabbitMqConnectionSettings.Username = "r/mhRXOYeJERTka7tzHfwA==";
    rabbitMqConnectionSettings.Password = "r/mhRXOYeJERTka7tzHfwA==";
    rabbitMqConnectionSettings.InitVector = "3p2ra5ux5e357t2i";
    rabbitMqConnectionSettings.PassPhrase = "4l146t34556422ny";
    builder.Services.Configure<RabbitMqConnectionSettings>((IConfiguration)rabbitMqConnectionSettings);
 
     */
}