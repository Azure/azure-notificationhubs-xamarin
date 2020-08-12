using System;
using Android.Content;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// Implementation of a NotificationListener for Android to receive messages.
    /// </summary>
    public class NotificationListener : Java.Lang.Object, INotificationListener
    {
        /// <summary>
        /// Invoked when a push notification has been received with the context and message.
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="message">The message containing the properties.</param>
        public void OnPushNotificationReceived(Context context, INotificationMessage message)
        {
            OnNotificationMessageReceivedAction?.Invoke(message);
        }

        /// <summary>
        /// The action to invoke when a message is received.
        /// </summary>
        public Action<INotificationMessage> OnNotificationMessageReceivedAction;
    }
}