namespace Microsoft.Azure.NotificationHubs.Client
{
    public interface IInstallationEnrichmentAdapter
    {
        void EnrichInstallation(Installation installation);
    }
}
