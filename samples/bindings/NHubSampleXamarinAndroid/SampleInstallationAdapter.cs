
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamarinAndroid
{
    public class SampleInstallationAdapter : Java.Lang.Object, IInstallationAdapter
    {
        public void SaveInstallation(Installation installation,
            IInstallationAdapterListener installationAdapterListener,
            IInstallationAdapterErrorListener installationAdapterErrorListener)
        {
            // Save to your own backend

            // Call if successfully saved
            installationAdapterListener.OnInstallationSaved(installation);
        }
    }
}
