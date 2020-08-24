using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// This class represents an Azure Notification Hubs Installation.
    /// </summary>
    public class Installation
    {
        /// <summary>
        /// The ID for the installation.
        /// </summary>
        public string InstallationId { get; set; }

        /// <summary>
        /// The PNS specific push channel for the installation.
        /// </summary>
        public string PushChannel { get; set; }

        /// <summary>
        /// The expiration time for the installation.
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Gets or sets the user ID for the installation.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The tags for the installation.
        /// </summary>
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// The templates for the installation.
        /// </summary>
        public IDictionary<string, InstallationTemplate> Templates { get; set; }
    }
}
