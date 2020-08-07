using System;
using UIKit;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamariniOS
{
    public class NotificationDelegate : MSNotificationHubDelegate
    {
        public NotificationDelegate()
        {
        }

        public override void DidReceivePushNotification(MSNotificationHub notificationHub, MSNotificationHubMessage message)
        {
            if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background)
            {
                Console.WriteLine($"Message received in the background with title {message.Title} and body {message.Body}");
            }
            else
            {
                Console.WriteLine($"Message received in the foreground with title {message.Title} and body {message.Body}");
            }
        }
    }
}
