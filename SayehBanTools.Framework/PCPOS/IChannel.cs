namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>کانال ارتباطی با کارت‌خوان (پورت سریال یا شبکه)</summary>
    public interface IChannel
    {
        bool IsOpen { get; set; }
        void Init(string[] para);
        bool Open();
        bool Close();
        int SendCommand(string command);
        string GetResponse();
        void DiscardInBuffer();
        void DiscardOutBuffer();
    }
}