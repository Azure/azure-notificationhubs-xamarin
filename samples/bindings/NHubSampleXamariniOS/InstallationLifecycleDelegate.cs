using System;
using Foundation;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamariniOS
{
    public class InstallationLifecycleDelegate : MSInstallationLifecycleDelegate
    {
        public InstallationLifecycleDelegate()
        {
        }

        public override void DidFailToSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation, NSError error)
        {
            Console.WriteLine($"Save installation failed with exception: {error.LocalizedDescription}");
        }

        public override void DidSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation)
        {
            Console.WriteLine($"Installation successfully saved with Installation ID: {installation.InstallationId}");
        }
    }
}
