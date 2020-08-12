using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// The EventArgs for when an installation has been saved successfully.
    /// </summary>
    public class InstallationSavedEventArgs : EventArgs
    {
        /// <summary>
        /// The installation that was saved to the backend.
        /// </summary>
        public Installation Installation { get; set; }
    }
}
