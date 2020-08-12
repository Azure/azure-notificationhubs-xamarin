using System;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// An implementation of the MSNotificationHubDelegate to intercept messages from APNS.
    /// </summary>
    public class NotificationHubMessageDelegate : MSNotificationHubDelegate
    {
        /// <summary>
        /// Invoked when a push notification has been received with the notification hub and message.
        /// </summary>
        /// <param name="notificationHub">The current NotificationHub instance.</param>
        /// <param name="message">The message from APNS.</param>
        public override void DidReceivePushNotification(MSNotificationHub notificationHub, MSNotificationHubMessage message)
        {
            OnNotificationMessageReceivedAction?.Invoke(message);
        }

        /// <summary>
        /// An action to invoke when a message is received via DidReceivePushNotification.
        /// </summary>
        public Action<MSNotificationHubMessage> OnNotificationMessageReceivedAction { get; set; }
    }
}
