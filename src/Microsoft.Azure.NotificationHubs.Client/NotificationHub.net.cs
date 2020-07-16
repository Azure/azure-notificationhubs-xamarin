using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public partial class NotificationHub
    {
        static string PlatformPushChannel => throw new NotImplementedException();
        static void PlatformSaveInstallation() => throw new NotImplementedException();
        static void PlatformInitialize(string connectionString, string hubName) => throw new NotImplementedException();

        #region Tags

        static bool PlatformAddTag(string tag) => default;
        static bool PlatformAddTags(string[] tags) => default;
        static void PlatformClearTags() => throw new NotImplementedException();
        static bool PlatformRemoveTag(string tag) => default;
        static bool PlatformRemoveTags(string[] tags) => default;
        static string[] PlatformGetTags() => default;

        #endregion

        #region Templates

        static void PlatformSetTemplate(string name, InstallationTemplate template) => throw new NotImplementedException();
        static void PlatformRemoveTemplate(string name) => throw new NotImplementedException();
        static InstallationTemplate PlatformGetTemplate(string name) => throw new NotImplementedException();

        #endregion
    }
}
