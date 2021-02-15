using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Yeastar.SmsApiClient.Events;

namespace Yeastar.SmsApiClient
{
    public class YeastarClient : IYeastarClient
    {
        private static readonly string SMS_RECEIVED_REGEX = "Event: ReceivedSMS\r\nPrivilege: (.+)\r\nID: (.+|)\r\nGsmSpan: (.+)\r\nSender: (.+)\r\nRecvtime: (.+)\r\nIndex: (.+)\r\nTotal: (.+)\r\nSmsc: (.+)\r\nContent: (.+)\r\n--END SMS EVENT--";
        private static readonly string SMS_SENT_REGEX = "Event: UpdateSMSSend\r\nPrivilege: (.+)\r\nID: (.+)\r\nSmsc: (.+)\r\nStatus: (.+)\r\n--END SMS EVENT--";
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        System.Threading.Thread thread;
        public YeastarClient(string address, int port)
        {
            client = new TcpClient(address, port);
            stream = client.GetStream();
            thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InternalTrhread));

        }
        private static readonly object _lock = new object();
        private void InternalTrhread(object obj)
        {
            while (true)
            {
                lock (_lock)
                {
                    using (var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true))
                    {
                        reader.BaseStream.ReadTimeout = 2000;
                        try
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            string text;
                            while (!string.IsNullOrEmpty((text = reader.ReadLine())))
                            {
                                stringBuilder.AppendLine(text);
                            }
                            var message = stringBuilder.ToString();

                            if (System.Text.RegularExpressions.Regex.IsMatch(message, SMS_SENT_REGEX))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(message, SMS_SENT_REGEX);
                                if (SmsUpdateEvent != null)
                                {
                                    SmsUpdateEvent(this, new UpdateSMSSendEventArgs(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, int.Parse(match.Groups[4].Value)));
                                }
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(message, SMS_RECEIVED_REGEX))
                            {

                                var match = System.Text.RegularExpressions.Regex.Match(message, SMS_RECEIVED_REGEX);
                                if (SmsReceivedEvent != null)
                                {
                                    SmsReceivedEvent(this, new ReceivedSMSEventArgs(match.Groups[1].Value, match.Groups[2].Value, int.Parse(match.Groups[3].Value), match.Groups[4].Value, DateTime.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value), match.Groups[8].Value, WebUtility.UrlDecode(match.Groups[9].Value)));
                                }
                            }
                        }
                        catch (System.IO.IOException ex)
                        {

                        }
                    }
                }
            }
        }

        public event EventHandler<UpdateSMSSendEventArgs> SmsUpdateEvent;
        public event EventHandler<ReceivedSMSEventArgs> SmsReceivedEvent;

        public void Dispose()
        {
            thread.Abort();
            client.GetStream().Dispose();
            client.Dispose();
        }

        public void LogIn(string username, string password)
        {
            using (var writter = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true))
            {
                writter.Write($"Action: Login\r\nUsername: {username}\r\nSecret: {password}\r\n\r\n");
            }
            var connectedMessage = ReadData();
            if (!connectedMessage.Contains("Authentication accepted"))
            {
                throw new UnauthorizedAccessException($"Cannot authenticate user: '{username}'");
            }
            thread.Start();
        }

        public void SendSms(int spanSimSlot, string phoneNumber, string message, string messageId)
        {
            lock (_lock)
            {
                using (var writter = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true))
                {
                    writter.Write($"Action: smscommand\r\ncommand: gsm send sms {spanSimSlot} {phoneNumber} \"{message}\" {messageId}\r\n\r\n");
                }

            }
        }

        public string ShowSpans()
        {
            lock (_lock)
            {
                using (var writter = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true))
                {
                    writter.Write("Action: smscommand\r\ncommand: gsm show spans\r\n\r\n");

                }
                var data = ReadData();
                return data;
            }
        }


        private string ReadData()
        {

            using (var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true))
            {
                StringBuilder stringBuilder = new StringBuilder();
                string text;
                while (!string.IsNullOrEmpty((text = reader.ReadLine())))
                {
                    stringBuilder.AppendLine(text);
                }
                return stringBuilder.ToString();
            }
        }
    }
}
