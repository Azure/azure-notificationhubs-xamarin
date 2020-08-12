namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// This represents a way to enrich the current installation before it is sent to the back-end.
    /// </summary>
    public interface IInstallationEnrichmentAdapter
    {
        /// <summary>
        /// Enriches an installation before being sent to the back-end.
        /// </summary>
        /// <param name="installation">The installation to enrich.</param>
        void EnrichInstallation(Installation installation);
    }
}
