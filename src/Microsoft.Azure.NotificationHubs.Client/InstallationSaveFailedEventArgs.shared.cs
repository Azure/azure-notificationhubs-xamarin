using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class InstallationSaveFailedEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
