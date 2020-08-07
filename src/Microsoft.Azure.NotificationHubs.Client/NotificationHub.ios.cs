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
        static readonly InstallationEnrichmentDelegate _installationEnrichmentDelegate = new InstallationEnrichmentDelegate();
        static readonly InstallationManagementDelegate _installationManagementDelegate = new InstallationManagementDelegate();
        static IInstallationManagementAdapter s_installationManagementAdapter;

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

            _installationEnrichmentDelegate.OnEnrichInstallation = installation =>
            {
                if (s_enrichmentAdapter != null)
                {
                    var convertedInstallation = installation.ToInstallation();
                    s_enrichmentAdapter?.EnrichInstallation(convertedInstallation);
                    convertedInstallation.CopyToNativeInstallation(installation);
                }
            };

            _installationManagementDelegate.OnUpsertInstallation = (installation, completionHandler) =>
            {
                var convertedInstallation = installation.ToInstallation();

                void OnSuccess(Installation i)
                {
                    completionHandler(null);
                }

                void OnError(Exception exception)
                {
                    var error = new NSError(new NSString("WindowsAzureMessaging"), -1);
                    error.UserInfo.SetValueForKey(new NSString(exception.Message), NSError.LocalizedDescriptionKey);
                    completionHandler(error);
                }

                s_installationManagementAdapter.SaveInstallation(convertedInstallation, OnSuccess, OnError);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName)
        {
            MSNotificationHub.SetLifecycleDelegate(_installationLifecycleDelegate);
            MSNotificationHub.SetDelegate(_delegate);
            MSNotificationHub.Start(connectionString, hubName);
            
        }

        static void PlatformInitialize(IInstallationManagementAdapter installationManagementAdapter)
        {
            s_installationManagementAdapter = installationManagementAdapter;
            MSNotificationHub.Start(_installationManagementDelegate);
            MSNotificationHub.SetLifecycleDelegate(_installationLifecycleDelegate);
            MSNotificationHub.SetDelegate(_delegate);
        }

        static void PlatformSetEnricher()
        {
            MSNotificationHub.SetEnrichmentDelegate(_installationEnrichmentDelegate);
        }

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
        public static void CopyToNativeInstallation(this Installation installation, MSInstallation nativeInstallation)
        {
            nativeInstallation.InstallationId = installation.InstallationId;
            if (installation.ExpirationTime != null)
            {
                nativeInstallation.ExpirationTime = FromDateTime(installation.ExpirationTime.Value);
            }

            nativeInstallation.PushChannel = installation.PushChannel;

            if (installation.Tags?.Count > 0)
            {
                foreach (var tag in installation.Tags)
                {
                    nativeInstallation.AddTag(tag);
                }
            }

            if (installation.Templates?.Count > 0)
            {
                foreach (var (key, value) in installation.Templates)
                {
                    nativeInstallation.SetTemplate(value.ToNativeInstallationTemplate(), key);
                }
            }
        }

        public static MSInstallation ToNativeInstallation(this Installation installation)
        {
            var nativeInstallation = new MSInstallation();
            CopyToNativeInstallation(installation, nativeInstallation);
            return nativeInstallation;
        }

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

        static NSDate FromDateTime(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var delta = dateTime - epoch;
            return NSDate.FromTimeIntervalSince1970(delta.TotalSeconds);
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

    class InstallationEnrichmentDelegate : MSInstallationEnrichmentDelegate
    {
        public override void WillEnrichInstallation(MSNotificationHub notificationHub, MSInstallation installation)
        {
            OnEnrichInstallation?.Invoke(installation);
        }

        public Action<MSInstallation> OnEnrichInstallation;
    }

    class InstallationManagementDelegate : MSInstallationManagementDelegate
    {
        public override void WillUpsertInstallation(MSNotificationHub notificationHub, MSInstallation installation, NullableCompletionHandler completionHandler)
        {
            throw new NotImplementedException();
        }

        public Action<MSInstallation, NullableCompletionHandler> OnUpsertInstallation;
    }
}
