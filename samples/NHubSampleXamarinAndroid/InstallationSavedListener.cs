using System;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamarinAndroid
{
    public class InstallationSavedListener : Java.Lang.Object, IInstallationAdapterListener
    {
        public InstallationSavedListener()
        {
        }

        public void OnInstallationSaved(Installation installation)
        {
            Console.WriteLine($"Installation successfully saved with Installation ID: {installation.InstallationId}");
        }
    }
}
