using System;
using System.Xml;

namespace SayehBanTools.Framework.PCPOS
{
    /// <summary>
    /// پیاده‌سازی پروتکل CPML (XML) که پروتکل عمومی اکثر PC-POSهای ایرانی است.
    /// پورت‌شده از PosCommander اصلی Aramestan.
    /// </summary>
    public class CpmlPosProtocol : IPosProtocol
    {
        private const string NODE_ADDITIONAL = "cpml/data";
        private const string NODE_SUCCESS = "cpml/response/status/success";
        private const string NODE_ERRORCODE = "cpml/response/status/errorcode";
        private const string NODE_WARNINGCODE = "cpml/response/status/warningcode";
        private const string NODE_TERMINALID = "cpml/response/posinfo/terminalid";
        private const string NODE_MERCHANTID = "cpml/response/posinfo/merchant/id";
        private const string NODE_MERCHANTNAME = "cpml/response/posinfo/merchant/name";
        private const string NODE_SERIALNO = "cpml/response/posinfo/serialnumber";
        private const string NODE_RESPONSECODE = "cpml/response/transdetails/rescode";
        private const string NODE_SVC = "cpml/response/transdetails/svc";
        private const string NODE_PAN = "cpml/response/transdetails/pan";
        private const string NODE_AMOUNT = "cpml/response/transdetails/amount";
        private const string NODE_DATETIME = "cpml/response/transdetails/datetime";
        private const string NODE_STAN = "cpml/response/transdetails/stan";
        private const string NODE_RRN = "cpml/response/transdetails/rrn";
        private const string NODE_ENTRYTYPE = "cpml/response/transdetails/entrytype";

        private IChannel channel;

        public PosConnectionSettings Settings { get; private set; }

        public CpmlPosProtocol()
        {
            Settings = new PosConnectionSettings();
        }

        /// <summary>
        /// ساخت کانال مناسب از روی تنظیمات و تست باز/بسته شدن آن.
        /// </summary>
        public void Init(PosConnectionSettings settings)
        {
            Settings = settings ?? new PosConnectionSettings();
            if (Settings.ConnectionType == PosConnectionType.SerialPort)
                channel = new SerialPortChannel();
            else
                channel = new SocketChannel();

            if (Settings.ConnectionType == PosConnectionType.SerialPort)
            {
                channel.Init(new[]
                {
                    Settings.SerialPort,
                    Settings.BaudRate.ToString(),
                    Settings.SerialTimeout.ToString()
                });
            }
            else
            {
                channel.Init(new[]
                {
                    Settings.LanIp,
                    Settings.LanTimeout.ToString()
                });
            }

            Open();
            Close();
        }

        public Response Purchase(string additionalData, string amount, bool print, string carId, string carOwner, string subject)
        {
            int respId = RandomId();
            string command = string.Format(
                "<cpml><data>{0}</data><command><purchase><amount>{1}</amount><carid>{2}</carid><carholdername>{3}</carholdername><billsubject>{4}</billsubject><print>{5}</print></purchase><response><respid>{6}</respid><status/><posinfo/><transdetails/></response></command></cpml>",
                additionalData, amount, carId, carOwner, subject, print ? "yes" : "no", respId);
            return SendCommandAndGetResponse(command);
        }

        public Response BalanceInquiry(string additionalData, bool print)
        {
            int respId = RandomId();
            string command = string.Format(
                "<cpml><data>{0}</data><command><balance><print>{1}</print></balance><response><respid>{2}</respid><status/><transdetails/><posinfo/></response></command></cpml>",
                additionalData, print ? "yes" : "no", respId);
            return SendCommandAndGetResponse(command);
        }

        public Response BillPayment(string additionalData, string billId, string paymentId, bool print)
        {
            int respId = RandomId();
            string command = string.Format(
                "<cpml><data>{0}</data><command><billpayment><billid>{1}</billid><paymentid>{2}</paymentid><print>{3}</print></billpayment><response><respid>{4}</respid><status/><transdetails/><posinfo/></response></command></cpml>",
                additionalData, billId, paymentId, print ? "yes" : "no", respId);
            return SendCommandAndGetResponse(command);
        }

        public Response GetPosInfo()
        {
            int respId = RandomId(1000, 9999);
            string command = string.Format(
                "<cpml><command><response><respid>{0}</respid><status/><posinfo/></response></command></cpml>", respId);
            return SendCommandAndGetResponse(command);
        }

        public Response LastTransaction()
        {
            int respId = RandomId(1000, 9999);
            string command = string.Format(
                "<cpml><command><lasttrans/><response><respid>{0}</respid><status/><transdetails/><posinfo/></response></command></cpml>", respId);
            return SendCommandAndGetResponse(command);
        }

        public Response LastPcPosTransaction()
        {
            // دستور اختصاصی دستگاه‌های PC-POS برای آخرین تراکنش خود دستگاه
            return SendCommandAndGetResponse("R");
        }

        public Response PrintText(string text, Language lang)
        {
            int respId = RandomId();
            string command = string.Format(
                "<cpml><command><print><lang>{0}</lang><barcode>no</barcode><text>{1}</text></print><response><respid>{2}</respid><status /></response></command></cpml>",
                lang, text, respId);
            return SendCommandAndGetResponse(command);
        }

        public Response PrintBarcode(string barcode, Language lang)
        {
            int respId = RandomId();
            string command = string.Format(
                "<cpml><command><print><lang>{0}</lang><barcode>yes</barcode><text>{1}</text></print><response><respid>{2}</respid><status /></response></command></cpml>",
                lang, barcode, respId);
            return SendCommandAndGetResponse(command);
        }

        private static int RandomId(int min, int max)
        {
            return new Random((int)(DateTime.Now.Ticks / 1000)).Next(min, max);
        }

        private static int RandomId()
        {
            return RandomId(100000, 999999);
        }

        private Response SendCommandAndGetResponse(string command)
        {
            if (channel == null)
                return Fail(201);

            if (!Open())
                return Fail(201);

            try
            {
                if (channel.SendCommand(command) == 0)
                {
                    string raw = channel.GetResponse();
                    if (!string.IsNullOrEmpty(raw))
                    {
                        Response parsed = ParseResponse(raw);
                        return parsed ?? Fail(204);
                    }
                    return Fail(203);
                }
                return Fail(202);
            }
            finally
            {
                Close();
            }
        }

        private static Response Fail(int code)
        {
            return new Response { Success = false, ErrorCode = code };
        }

        private bool Open()
        {
            try
            {
                if (channel == null)
                    return false;
                if (!channel.IsOpen)
                    channel.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool Close()
        {
            try
            {
                if (channel != null && channel.IsOpen)
                    channel.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetNodeInfo(string responseXml, string node)
        {
            if (string.IsNullOrWhiteSpace(responseXml) || string.IsNullOrWhiteSpace(node))
                return "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseXml);
                XmlNodeList nodes = doc.SelectNodes(node);
                return nodes != null && nodes.Count > 0 ? nodes[0].InnerText : "";
            }
            catch (XmlException)
            {
                throw;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static Response ParseResponse(string responseXml)
        {
            try
            {
                Response response = new Response();
                string additional = GetNodeInfo(responseXml, NODE_ADDITIONAL);
                if (!string.IsNullOrEmpty(additional))
                    response.AdditionalData = additional;

                bool success = GetNodeInfo(responseXml, NODE_SUCCESS) == "yes";
                response.Success = success;

                if (!success)
                {
                    string error = GetNodeInfo(responseXml, NODE_ERRORCODE);
                    response.ErrorCode = !string.IsNullOrEmpty(error) ? int.Parse(error) : 200;
                }

                string warning = GetNodeInfo(responseXml, NODE_WARNINGCODE);
                if (!string.IsNullOrEmpty(warning))
                    response.WarningCode = int.Parse(warning);

                response.PosInformation = new PosInfo();
                string terminalId = GetNodeInfo(responseXml, NODE_TERMINALID);
                if (!string.IsNullOrEmpty(terminalId))
                    response.PosInformation.TerminalId = terminalId;
                string merchantName = GetNodeInfo(responseXml, NODE_MERCHANTNAME);
                if (!string.IsNullOrEmpty(merchantName))
                    response.PosInformation.MerchantName = merchantName;
                string serialNumber = GetNodeInfo(responseXml, NODE_SERIALNO);
                if (!string.IsNullOrEmpty(serialNumber))
                    response.PosInformation.SerialNumber = serialNumber;
                string merchantId = GetNodeInfo(responseXml, NODE_MERCHANTID);
                if (!string.IsNullOrEmpty(merchantId))
                    response.PosInformation.MerchantId = merchantId;

                if (success && !string.IsNullOrEmpty(GetNodeInfo(responseXml, NODE_RESPONSECODE)))
                {
                    response.TransactionInfo = new Transaction();
                    string rescode = GetNodeInfo(responseXml, NODE_RESPONSECODE);
                    if (!string.IsNullOrEmpty(rescode))
                        response.TransactionInfo.ResponseCode = rescode;
                    string svc = GetNodeInfo(responseXml, NODE_SVC);
                    if (!string.IsNullOrEmpty(svc))
                        response.TransactionInfo.SVC = svc;
                    string pan = GetNodeInfo(responseXml, NODE_PAN);
                    if (!string.IsNullOrEmpty(pan))
                        response.TransactionInfo.PAN = pan;
                    string amount = GetNodeInfo(responseXml, NODE_AMOUNT);
                    if (!string.IsNullOrEmpty(amount))
                        response.TransactionInfo.Amount = amount;
                    string dateTime = GetNodeInfo(responseXml, NODE_DATETIME);
                    if (!string.IsNullOrEmpty(dateTime))
                        response.TransactionInfo.DateTime = dateTime;
                    string stan = GetNodeInfo(responseXml, NODE_STAN);
                    if (!string.IsNullOrEmpty(stan))
                        response.TransactionInfo.Stan = stan;
                    string rrn = GetNodeInfo(responseXml, NODE_RRN);
                    if (!string.IsNullOrEmpty(rrn))
                        response.TransactionInfo.RRN = rrn;
                    string entryType = GetNodeInfo(responseXml, NODE_ENTRYTYPE);
                    if (!string.IsNullOrEmpty(entryType))
                        response.TransactionInfo.EntryType = entryType;
                }
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}