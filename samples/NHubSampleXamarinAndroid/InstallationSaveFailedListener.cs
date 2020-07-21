using System;
using Java.Lang;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamarinAndroid
{
    public class InstallationSaveFailedListener : Java.Lang.Object, IInstallationAdapterErrorListener
    {
        public InstallationSaveFailedListener()
        {
        }

        public void OnInstallationSaveError(Java.Lang.Exception javaException)
        {
            var exception = Throwable.FromException(javaException);
            Console.WriteLine($"Save installation failed with exception: {exception.Message}");
        }
    }
}
