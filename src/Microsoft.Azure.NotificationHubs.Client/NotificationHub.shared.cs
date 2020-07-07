using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        public static string PushChannel => PlatformPushChannel;

        public static EventHandler<NotificationMessageReceivedEventArgs> NotificationMessageReceived;

        public static void Start(string connectionString, string hubName)
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

        #region Templates

        public static void SetTemplate(string name, InstallationTemplate template) => PlatformSetTemplate(name, template);
        public static void RemoveTemplate(string name) => PlatformRemoveTemplate(name);
        public static InstallationTemplate GetTemplate(string name) => PlatformGetTemplate(name);

        #endregion

    }
}
