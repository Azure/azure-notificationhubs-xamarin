using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class InstallationTemplate
    {
        public string Body { get; set; }
        public IList<string> Tags { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
