using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamarinAndroid
{
    public class InstallationEnrichmentVisitor : Java.Lang.Object, IInstallationVisitor
    {
        public void VisitInstallation(Installation installation)
        {
            // Add a sample tag
            installation.AddTag("platform_XamarinAndroid");
        }
    }
}
