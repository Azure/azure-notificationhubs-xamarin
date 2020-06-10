using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        public static EventHandler<NotificationMessageReceivedEventArgs> NotificationMessageReceived;

        public static void Initialize(string connectionString, string hubName)
        {
            PlatformInitialize(connectionString, hubName);
        }

        #region Tags

        public static bool AddTag(string tag) => PlatformAddTag(tag);
        public static bool AddTags(string[] tags) => PlatformAddTags(tags);
        public static void ClearTags() => PlatformClearTags();
        public static bool RemoveTag(string tag) => PlatformRemoveTag(tag);
        public static bool RemoveTags(string[] tags) => PlatformRemoveTags(tags);
        public static string[] GetTags() => PlatformGetTags();

        #endregion

    }
}
