using System;
using System.Net.Sockets;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>کانال شبکه TCP؛ پورت پیش‌فرض CPML یعنی 16028 و خواندن پاسخ در بافر 20480 بایتی.</summary>
    public class SocketChannel : IChannel
    {
        private Socket socket;
        private string host = "";
        private int port = 16028;
        private int timeOut;

        public bool IsOpen { get; set; }

        public void Init(string[] para)
        {
            host = para[0];
            timeOut = int.Parse(para[1]) * 1000;
        }

        public bool Open()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = timeOut;
            socket.SendTimeout = timeOut;
            socket.Connect(host, port);
            IsOpen = true;
            return IsOpen;
        }

        public bool Close()
        {
            if (socket != null)
                socket.Close();
            IsOpen = false;
            return IsOpen;
        }

        public int SendCommand(string command)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(command);
                socket.Send(bytes, 0, bytes.Length, SocketFlags.None);
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }

        public string GetResponse()
        {
            try
            {
                byte[] bytes = new byte[20480];
                int count = socket.Receive(bytes, SocketFlags.None);
                return System.Text.Encoding.ASCII.GetString(bytes, 0, count);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void DiscardInBuffer() { }

        public void DiscardOutBuffer() { }
    }
}