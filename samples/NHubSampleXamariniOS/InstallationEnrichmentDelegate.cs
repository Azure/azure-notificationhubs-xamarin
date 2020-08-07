using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamariniOS
{
    public class InstallationEnrichmentDelegate : MSInstallationEnrichmentDelegate
    {
        public override void WillEnrichInstallation(MSNotificationHub notificationHub, MSInstallation installation)
        {
            installation.AddTag("platform_Xamarin");
        }
    }
}
