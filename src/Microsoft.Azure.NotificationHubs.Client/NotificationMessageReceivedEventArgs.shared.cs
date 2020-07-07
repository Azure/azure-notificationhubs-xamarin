using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class NotificationMessageReceivedEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public IDictionary<string, string> Data { get; set; }
    }
}
