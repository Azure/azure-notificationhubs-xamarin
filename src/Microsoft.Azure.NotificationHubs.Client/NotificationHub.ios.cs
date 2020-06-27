using System;
using System.Collections.Generic;
using Foundation;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string PlatformPushChannel {
            get {
                return MSNotificationHub.GetPushChannel();
            }
        }

        static readonly NotificationHubMessageDelegate _delegate = new NotificationHubMessageDelegate();

        static NotificationHub()
        {
            _delegate.OnNotificationMessageReceivedAction = message =>
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
            MSNotificationHub.Init(connectionString, hubName);
        }

        #region Tags

        static bool PlatformAddTag(string tag) => MSNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => MSNotificationHub.AddTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static void PlatformClearTags() => MSNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => MSNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => MSNotificationHub.RemoveTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static string[] PlatformGetTags() => ConvertNSArrayToArray(MSNotificationHub.GetTags());

        #endregion

        static string[] ConvertNSArrayToArray(NSArray nsArray)
        {
            var items = new List<string>();
            for (var i = 0; i < (int)nsArray.Count; i++)
            {
                var item = nsArray.GetItem<NSString>((nuint)i).ToString();
                items.Add(item);
            }

            return items.ToArray();
        }
    }
}
