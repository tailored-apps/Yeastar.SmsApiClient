using System;
using System.Collections.Generic;
using System.Text;

namespace Yeastar.SmsApiClient.Events
{
    public class UpdateSMSSendEventArgs
    {
        public UpdateSMSSendEventArgs(string privilege, string id, string smsc, int status)
        {
            this.Privilege = privilege;
            this.ID = id;
            this.Smsc = smsc;
            this.Status = status;
        }
        public string Privilege { get; private set; }
        public string ID { get; private set; }
        public string Smsc { get; private set; }
        public int Status { get; private set; }
    }
}
