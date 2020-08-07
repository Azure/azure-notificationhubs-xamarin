using ObjCRuntime;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamariniOS
{
    public class InstallationManagementDelegate : MSInstallationManagementDelegate
    {
        public override void WillDeleteInstallation(MSNotificationHub notificationHub, string installationId, NullableCompletionHandler completionHandler)
        {
            completionHandler(null);
        }

        public override void WillUpsertInstallation(MSNotificationHub notificationHub, MSInstallation installation, NullableCompletionHandler completionHandler)
        {
            // Save the installation to your own backend
            // Finish with a completion with error if one occurred, else null
            completionHandler(null);
        }
    }
}
