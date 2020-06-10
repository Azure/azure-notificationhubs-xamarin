using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class NotificationMessageReceivedEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
