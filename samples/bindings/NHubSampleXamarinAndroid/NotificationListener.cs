using System;
using Android.Content;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamarinAndroid
{
    public class SampleNotificationListener : Java.Lang.Object, INotificationListener
    {
        public SampleNotificationListener()
        {
        }

        public void OnPushNotificationReceived(Context context, INotificationMessage message)
        {
            Console.WriteLine($"Message received with title {message.Title} and body {message.Body}");
        }
    }
}
