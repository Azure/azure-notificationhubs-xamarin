using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Android.App;
using AndroidNotificationHub = WindowsAzure.Messaging.NotificationHubs.NotificationHub;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string PlatformPushChannel {
            get {
                return AndroidNotificationHub.PushChannel;
            }
        }

        static readonly NotificationListener _listener = new NotificationListener();

        static NotificationHub()
        {
            _listener.OnNotificationMessageReceivedAction = message =>
            {
                var args = new NotificationMessageReceivedEventArgs
                {
                    Title = message.Title,
                    Body = message.Body
                };

                NotificationMessageReceived?.Invoke(null, args);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName)
        {
            AndroidNotificationHub.Initialize((Application)Application.Context, hubName, connectionString);
        }

        #region Tags

        static bool PlatformAddTag(string tag) => AndroidNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => AndroidNotificationHub.AddTags(tags);
        static void PlatformClearTags() => AndroidNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => AndroidNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => AndroidNotificationHub.RemoveTags(tags);
        static string[] PlatformGetTags() => ConvertIterableToArray(AndroidNotificationHub.Tags);

        #endregion

        static string[] ConvertIterableToArray(Java.Lang.IIterable iterable)
        {
            var items = new List<string>();
            var iterator = iterable.Iterator();

            while (iterator.HasNext)
            {
                var item = (string)iterator.Next();
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}
