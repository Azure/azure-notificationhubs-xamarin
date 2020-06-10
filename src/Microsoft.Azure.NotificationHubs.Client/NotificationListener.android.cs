using System;
using Android.Content;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public class NotificationListener : Java.Lang.Object, INotificationListener
    {

        public void OnPushNotificationReceived(Context context, INotificationMessage message)
        {
            OnNotificationMessageReceivedAction?.Invoke(message);
        }

        public Action<INotificationMessage> OnNotificationMessageReceivedAction;
    }
}