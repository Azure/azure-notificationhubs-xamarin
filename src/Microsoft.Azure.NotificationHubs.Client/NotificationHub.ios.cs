using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using WindowsAzure.Messaging.NotificationHubs;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string PlatformPushChannel => MSNotificationHub.GetPushChannel();
        public static void PlatformSaveInstallation() => MSNotificationHub.WillSaveInstallation();

        static readonly NotificationHubMessageDelegate _delegate = new NotificationHubMessageDelegate();
        static readonly InstallationLifecycleDelegate _installationLifecycleDelegate = new InstallationLifecycleDelegate();

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

            _installationLifecycleDelegate.OnInstallationSaved = installation =>
            {
                var args = new InstallationSavedEventArgs
                {
                    Installation = installation.ToInstallation()
                };

                InstallationSaved?.Invoke(null, args);
            };

            _installationLifecycleDelegate.OnInstallationSaveFailed = exception =>
            {
                var args = new InstallationSaveFailedEventArgs
                {
                    Exception = new Exception(exception.LocalizedDescription)
                };

                InstallationSaveFailed?.Invoke(null, args);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName) => MSNotificationHub.Start(connectionString, hubName);

        #region iOS Initialization

        public static void RegisteredForRemoteNotifications(NSData deviceToken) => MSNotificationHub.DidRegisterForRemoteNotifications(deviceToken);
        public static void FailedToRegisterForRemoteNotifications(NSError error) => MSNotificationHub.DidFailToRegisterForRemoteNotifications(error);
        public static void DidReceiveRemoteNotification(NSDictionary userInfo) => MSNotificationHub.DidReceiveRemoteNotification(userInfo);
        

        #endregion

        #region Tags

        static bool PlatformAddTag(string tag) => MSNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => MSNotificationHub.AddTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static void PlatformClearTags() => MSNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => MSNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => MSNotificationHub.RemoveTags((NSArray<NSString>)NSArray.FromStrings(tags));
        static string[] PlatformGetTags() => MSInstallationExtensions.GetTags(MSNotificationHub.GetTags()).ToArray();

        #endregion

        #region Templates

        static void PlatformSetTemplate(string name, InstallationTemplate template) => MSNotificationHub.SetTemplate(template.ToNativeInstallationTemplate(), name);
        static void PlatformRemoveTemplate(string name) => MSNotificationHub.RemoveTemplate(name);

        static InstallationTemplate PlatformGetTemplate(string name)
        {
            var nativeTemplate = MSNotificationHub.GetTemplate(name);
            if (nativeTemplate == null) return default;

            return nativeTemplate.ToInstallationTemplate();
        }

        #endregion

    }

    static class MSInstallationExtensions
    {
        public static Installation ToInstallation(this MSInstallation nativeInstallation)
        {
            var installation = new Installation
            {
                InstallationId = nativeInstallation.InstallationId,
                PushChannel = nativeInstallation.PushChannel
            };

            if (nativeInstallation.ExpirationTime != null)
            {
                installation.ExpirationTime = FromUnixTime(nativeInstallation.ExpirationTime.SecondsSince1970);
            }

            if (nativeInstallation.Tags?.Count > 0)
            {
                installation.Tags = ((NSSet)nativeInstallation.Tags).Select(tag => tag.ToString()).ToList();
            }

            if (nativeInstallation.Templates?.Count > 0)
            {
                installation.Templates = ((NSDictionary)nativeInstallation.Templates).ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => ((MSInstallationTemplate)kvp.Value).ToInstallationTemplate());
            }

            return installation;
        }

        static DateTime FromUnixTime(double unixTimeSeconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeSeconds);
        }

        public static InstallationTemplate ToInstallationTemplate(this MSInstallationTemplate nativeTemplate)
        {
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

            if (nativeTemplate.Headers?.Count > 0)
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

        public static MSInstallationTemplate ToNativeInstallationTemplate(this InstallationTemplate template)
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

            return nativeTemplate;
        }

        public static List<string> GetTags(NSArray nsArray)
        {
            var items = new List<string>();
            for (var i = 0; i < (int)nsArray.Count; i++)
            {
                var item = nsArray.GetItem<NSString>((nuint)i).ToString();
                items.Add(item);
            }

            return items;
        }
    }

    class InstallationLifecycleDelegate : MSInstallationLifecycleDelegate
    {
        public override void DidFailToSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation, NSError error)
        {
            OnInstallationSaveFailed?.Invoke(error);
        }

        public override void DidSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation)
        {
            OnInstallationSaved?.Invoke(installation);
        }

        public Action<NSError> OnInstallationSaveFailed;
        public Action<MSInstallation> OnInstallationSaved;
    }
}
