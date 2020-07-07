using System.Collections.Generic;
using Android.App;
using AndroidNotificationHub = WindowsAzure.Messaging.NotificationHubs.NotificationHub;
using AndroidInstallationTemplate = WindowsAzure.Messaging.NotificationHubs.InstallationTemplate;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static readonly NotificationListener _listener = new NotificationListener();

        static NotificationHub()
        {
            _listener.OnNotificationMessageReceivedAction = message =>
            {
                
                var args = new NotificationMessageReceivedEventArgs
                {
                    Title = message.Title,
                    Body = message.Body,
                    Data = message.Data
                };

                NotificationMessageReceived?.Invoke(null, args);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName) => AndroidNotificationHub.Initialize((Application)Application.Context, hubName, connectionString);

        #region Tags

        static bool PlatformAddTag(string tag) => AndroidNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => AndroidNotificationHub.AddTags(tags);
        static void PlatformClearTags() => AndroidNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => AndroidNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => AndroidNotificationHub.RemoveTags(tags);
        static string[] PlatformGetTags() => ConvertIterableToArray(AndroidNotificationHub.Tags);

        #endregion

        #region Templates

        static void PlatformSetTemplate(string name, InstallationTemplate template)
        {
            var nativeTemplate = new AndroidInstallationTemplate
            {
                Body = template.Body
            };

            if (template.Tags?.Count > 0)
            {
                foreach (var tag in template.Tags)
                {
                    nativeTemplate.AddTag(tag);
                }
            }

            if (template.Headers?.Count > 0)
            {
                foreach (var (key, value) in template.Headers)
                {
                    nativeTemplate.SetHeader(key, value);
                }
            }

            AndroidNotificationHub.SetTemplate(name, nativeTemplate);
        }

        static void PlatformRemoveTemplate(string name) => AndroidNotificationHub.RemoveTemplate(name);

        static InstallationTemplate PlatformGetTemplate(string name)
        {
            var nativeTemplate = AndroidNotificationHub.GetTemplate(name);
            if (nativeTemplate == null) return default;

            var template = new InstallationTemplate
            {
                Body = nativeTemplate.Body
            };

            if (nativeTemplate.Tags != null)
            {
                var tags = new List<string>();
                var iterator = nativeTemplate.Tags.Iterator();
                while (iterator.HasNext)
                {
                    tags.Add((string)iterator.Next());
                }

                template.Tags = tags;
            }

            // TODO: Add headers once supported in native
            // foreach (var kvp in nativeTemplate.Headers)

            return template;
        }

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
