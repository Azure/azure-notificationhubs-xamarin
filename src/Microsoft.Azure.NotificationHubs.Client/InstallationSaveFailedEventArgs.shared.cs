using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// The EventArgs for when an installation save has failed.
    /// </summary>
    public class InstallationSaveFailedEventArgs : EventArgs
    {
        /// <summary>
        /// The exception from saving the installation.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
