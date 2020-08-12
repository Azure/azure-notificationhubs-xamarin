using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// The EventArgs received when a push notification has been received.
    /// </summary>
    public class NotificationMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The title of the push notification.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body of the push notification.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The data from the push notification.
        /// </summary>
        public IDictionary<string, string> Data { get; set; }
    }
}
