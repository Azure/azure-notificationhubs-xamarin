using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static readonly NotificationHubMessageDelegate _delegate = new NotificationHubMessageDelegate();

        static NotificationHub()
        {
            _delegate.OnNotificationMessageReceivedAction = message =>
            {
                var args = new NotificationMessageReceivedEventArgs
                {
                    Title = message.Title,
                    Body = message.Body,
                    Data = ((NSDictionary)message.UserInfo).ToDictionary(i => i.Key.ToString(), i => i.Value.ToString())
                };

                NotificationMessageReceived?.Invoke(null, args);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName)
        {
            MSNotificationHub.Start(connectionString, hubName);
        }

        #region iOS Initialization

        public static void RegisteredForRemoteNotifications(NSData deviceToken) => MSNotificationHub.DidRegisterForRemoteNotifications(deviceToken);
        public static void FailedToRegisterForRemoteNotifications(NSError error) => MSNotificationHub.DidFailToRegisterForRemoteNotifications(error);
        public static void DidReceiveRemoteNotification(NSDictionary userInfo) => MSNotificationHub.DidReceiveRemoteNotification(userInfo);

        // TODO: WillSaveInstallation
        // public static void WillSaveInstallation();
        #endregion

        #region Tags

        static bool PlatformAddTag(string tag) => MSNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => MSNotificationHub.AddTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static void PlatformClearTags() => MSNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => MSNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => MSNotificationHub.RemoveTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static string[] PlatformGetTags() => ConvertNSArrayToArray(MSNotificationHub.GetTags());

        #endregion

        #region Templates

        static void PlatformSetTemplate(string name, InstallationTemplate template)
        {
            var nativeTemplate = new MSInstallationTemplate
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
                    nativeTemplate.SetHeader(value, key);
                }
            }



            MSNotificationHub.SetTemplate(nativeTemplate, name);
        }

        static void PlatformRemoveTemplate(string name) => MSNotificationHub.RemoveTemplate(name);

        static InstallationTemplate PlatformGetTemplate(string name)
        {
            var nativeTemplate = MSNotificationHub.GetTemplate(name);
            if (nativeTemplate == null) return default;

            var template = new InstallationTemplate
            {
                Body = nativeTemplate.Body
            };

            if (nativeTemplate.Tags?.Count > 0)
            {
                var tags = new List<string>();

                foreach (var nativeTag in nativeTemplate.Tags)
                {
                    tags.Add(nativeTag.ToString());
                }

                template.Tags = tags;
            }

            if (nativeTemplate?.Headers.Count > 0)
            {
                var headers = new Dictionary<string, string>();

                foreach (var (key, value) in nativeTemplate.Headers)
                {
                    headers.Add(key.ToString(), value.ToString());
                }

                template.Headers = headers;
            }


            return template;
        }

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
