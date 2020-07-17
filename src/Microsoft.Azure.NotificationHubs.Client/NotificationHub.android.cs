using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Java.Lang;
using AndroidInstallation = WindowsAzure.Messaging.NotificationHubs.Installation;
using AndroidInstallationTemplate = WindowsAzure.Messaging.NotificationHubs.InstallationTemplate;
using AndroidNotificationHub = WindowsAzure.Messaging.NotificationHubs.NotificationHub;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string PlatformPushChannel => AndroidNotificationHub.PushChannel;
        static void PlatformSaveInstallation() => AndroidNotificationHub.BeginInstallationUpdate();

        static readonly NotificationListener _listener = new NotificationListener();
        static readonly InstallationAdapterListener _installationSavedListener = new InstallationAdapterListener();
        static readonly InstallationAdapterErrorListener _installationErrorListener = new InstallationAdapterErrorListener();

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

            _installationSavedListener.OnInstallationSavedAction = installation =>
            {
                var args = new InstallationSavedEventArgs
                {
                    Installation = installation.ToInstallation()
                };

                InstallationSaved?.Invoke(null, args);
            };

            _installationErrorListener.OnInstallationSaveErrorAction = exception =>
            {
                var args = new InstallationSaveFailedEventArgs
                {
                    Exception = Throwable.ToException(exception)
                };

                InstallationSaveFailed?.Invoke(null, args);
            };
        }

        static void PlatformInitialize(string connectionString, string hubName)
        {
            AndroidNotificationHub.Initialize((Application)Application.Context, hubName, connectionString);
            AndroidNotificationHub.SetListener(_listener);
            AndroidNotificationHub.SetInstallationSavedListener(_installationSavedListener);
            AndroidNotificationHub.SetInstallationSaveFailureListener(_installationErrorListener);
        }

        #region Tags

        static bool PlatformAddTag(string tag) => AndroidNotificationHub.AddTag(tag);
        static bool PlatformAddTags(string[] tags) => AndroidNotificationHub.AddTags(tags);
        static void PlatformClearTags() => AndroidNotificationHub.ClearTags();
        static bool PlatformRemoveTag(string tag) => AndroidNotificationHub.RemoveTag(tag);
        static bool PlatformRemoveTags(string[] tags) => AndroidNotificationHub.RemoveTags(tags);
        static string[] PlatformGetTags() => InstallationExtensions.GetTags(AndroidNotificationHub.Tags).ToArray();

        #endregion

        #region Templates

        static void PlatformSetTemplate(string name, InstallationTemplate template) => AndroidNotificationHub.SetTemplate(name, template.ToNativeTemplate());
        static void PlatformRemoveTemplate(string name) => AndroidNotificationHub.RemoveTemplate(name);

        static InstallationTemplate PlatformGetTemplate(string name)
        {
            var nativeTemplate = AndroidNotificationHub.GetTemplate(name);
            if (nativeTemplate == null) return default;

            return nativeTemplate.ToInstallationTemplate();
        }

        #endregion
    }

    static class InstallationExtensions
    {
        public static Installation ToInstallation(this AndroidInstallation nativeInstallation)
        {
            var installation = new Installation
            {
                InstallationId = nativeInstallation.InstallationId,
                PushChannel = nativeInstallation.PushChannel
            };

            if (nativeInstallation.Expiration != null)
            {
                installation.ExpirationTime = FromUnixTime(nativeInstallation.Expiration.Time);
            }

            if (installation.Tags?.Count > 0)
            {
                installation.Tags = GetTags(nativeInstallation.Tags);
            }

            if (installation.Templates?.Count > 0)
            {
                installation.Templates = nativeInstallation.Templates.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToInstallationTemplate());
            }   

            return installation;
        }

        static DateTime FromUnixTime(long unixTimeMillis)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTimeMillis);
        }

        public static List<string> GetTags(IIterable nativeTags)
        {
            var tags = new List<string>();
            var iterator = nativeTags.Iterator();
            while (iterator.HasNext)
            {
                tags.Add((string)iterator.Next());
            }

            return tags;
        }

        public static InstallationTemplate ToInstallationTemplate(this AndroidInstallationTemplate nativeTemplate)
        {
            var template = new InstallationTemplate
            {
                Body = nativeTemplate.Body
            };

            if (nativeTemplate.Tags != null)
            {
                template.Tags = GetTags(nativeTemplate.Tags);
            }

            // TODO: Add headers once supported in native
            // foreach (var kvp in nativeTemplate.Headers)

            return template;
        }

        public static AndroidInstallationTemplate ToNativeTemplate(this InstallationTemplate template)
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

            return nativeTemplate;
        }
    }

    class InstallationAdapterListener : Java.Lang.Object, WindowsAzure.Messaging.NotificationHubs.IInstallationAdapterListener
    {
        public void OnInstallationSaved(AndroidInstallation installation)
        {
            OnInstallationSavedAction?.Invoke(installation);
        }

        public Action<AndroidInstallation> OnInstallationSavedAction;
    }

    class InstallationAdapterErrorListener : Java.Lang.Object, WindowsAzure.Messaging.NotificationHubs.IInstallationAdapterErrorListener
    {
        public void OnInstallationSaveError(Java.Lang.Exception exception)
        {
            OnInstallationSaveErrorAction?.Invoke(exception);
        }

        public Action<Java.Lang.Exception> OnInstallationSaveErrorAction;
    }
}
