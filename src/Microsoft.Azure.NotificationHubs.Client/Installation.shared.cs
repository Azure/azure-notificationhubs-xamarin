using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class Installation
    {
        public string InstallationId { get; set; }
        public string PushChannel { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public ICollection<string> Tags { get; set; }
        public IDictionary<string, InstallationTemplate> Templates { get; set; }
    }
}
