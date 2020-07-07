using System;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class NotificationHubMessageDelegate : MSNotificationHubDelegate
    {
        public override void DidReceivePushNotification(MSNotificationHub notificationHub, MSNotificationHubMessage message)
        {
            OnNotificationMessageReceivedAction?.Invoke(message);
        }

        public Action<MSNotificationHubMessage> OnNotificationMessageReceivedAction { get; set; }
    }
}
