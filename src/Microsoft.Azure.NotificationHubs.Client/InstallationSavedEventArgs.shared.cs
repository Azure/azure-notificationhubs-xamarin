using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class InstallationSavedEventArgs : EventArgs
    {
        public Installation Installation { get; set; }
    }
}
