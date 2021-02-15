using System;
using System.Collections.Generic;
using System.Text;
using Yeastar.SmsApiClient.Events;

namespace Yeastar.SmsApiClient
{
    public interface IYeastarClient : IDisposable
    {
        public event EventHandler<UpdateSMSSendEventArgs> SmsUpdateEvent;
        public event EventHandler<ReceivedSMSEventArgs> SmsReceivedEvent;

        void LogIn(string username, string password);
        void SendSms(int simCardSlot, string phoneNumber, string message, string messageId);
        string ShowSpans();
    }
}
