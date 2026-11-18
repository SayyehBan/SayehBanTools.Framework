using System;
using System.IO.Ports;
using System.Threading;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// کانال پورت سریال با فریمینگ STX(0x02)/طول دو بایتی/LRC/ETX(0x03) و دریافت ACK(0x05)؛ نرخ باود پیش‌فرض 38400.
    /// </summary>
    public class SerialPortChannel : IChannel
    {
        private readonly SerialPort serialPort;
        private readonly byte[] frame = new byte[6];

        public SerialPortChannel()
        {
            serialPort = new SerialPort();
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.ReadBufferSize = 10240;
            serialPort.WriteTimeout = 120000;
        }

        public string Port { get; set; } = "";
        public int BaudRate { get; set; }
        public int TimeOut { get; set; }
        public bool IsOpen { get; set; }

        public void Init(string[] para)
        {
            Port = para[0];
            BaudRate = int.Parse(para[1]);
            TimeOut = int.Parse(para[2]) * 1000;
            serialPort.BaudRate = BaudRate;
            serialPort.ReadTimeout = TimeOut;
            serialPort.WriteTimeout = TimeOut;
            serialPort.PortName = Port;
        }

        public bool Open()
        {
            serialPort.Open();
            IsOpen = true;
            return IsOpen;
        }

        public bool Close()
        {
            serialPort.Close();
            IsOpen = false;
            return IsOpen;
        }

        public void DiscardInBuffer()
        {
            serialPort.DiscardInBuffer();
        }

        public void DiscardOutBuffer()
        {
            serialPort.DiscardOutBuffer();
        }

        /// <summary>ارسال دستور با حداکثر ۳ بار تلاش تا دریافت ACK (0x05). خروجی 0 یعنی موفق.</summary>
        public int SendCommand(string command)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(command);
                byte lrc = 0;
                frame[0] = 2;                       // STX
                frame[1] = 1;
                frame[2] = (byte)(bytes.Length % 256);
                frame[3] = (byte)(bytes.Length / 256);
                foreach (byte b in bytes)
                    lrc ^= b;
                frame[4] = lrc;
                frame[5] = 3;                       // ETX

                int attempt;
                for (attempt = 0; attempt < 3; attempt++)
                {
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();
                    serialPort.Write(frame, 0, 4);
                    serialPort.Write(bytes, 0, bytes.Length);
                    serialPort.Write(frame, 4, 2);
                    if (serialPort.ReadByte() == 5) // ACK
                    {
                        Thread.Sleep(1000);
                        break;
                    }
                }
                if (attempt == 3)
                    return 1; // بدون ACK
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
                // سرِ فریم: STX و بایت بعدی(1) سپس طول دو بایتی
                while (serialPort.ReadByte() != 2 || serialPort.ReadByte() != 1)
                {
                }
                int length = serialPort.ReadByte();
                length += serialPort.ReadByte() * 256;
                byte[] buffer = new byte[length];
                for (int i = 0; i < length; i++)
                    buffer[i] = (byte)serialPort.ReadByte();
                return System.Text.Encoding.ASCII.GetString(buffer);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}