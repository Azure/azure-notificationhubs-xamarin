using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// The Installation Template for Installations.
    /// </summary>
    public class InstallationTemplate
    {
        /// <summary>
        /// The body of the template.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The tags for the template.
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// The headers for the template.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
    }
}
