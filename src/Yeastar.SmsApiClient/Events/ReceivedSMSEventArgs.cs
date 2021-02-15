using System;
using System.Collections.Generic;
using System.Text;

namespace Yeastar.SmsApiClient.Events
{
    public class ReceivedSMSEventArgs
    {
        public ReceivedSMSEventArgs(string privilege, string id, int gsmSpan, string sender, DateTime recvtime, int index, int total, string smsc, string content)
        {
            this.Privilege = privilege;
            this.ID = id;
            this.GsmSpan = gsmSpan;
            this.Sender = sender;
            this.Recvtime = recvtime;
            this.Index = index;
            this.Total = total;
            this.Smsc = smsc;
            this.Content = content;
        }
        public string Privilege { get; private set; }
        public string ID { get; private set; }
        public int GsmSpan { get; private set; }
        public string Sender { get; private set; }
        public DateTime Recvtime { get; private set; }
        public int Index { get; private set; }
        public int Total { get; private set; }
        public string Smsc { get; private set; }
        public string Content { get; private set; }
    }
}
