using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// Handles the interactions with Azure Notification Hubs
    /// </summary>
    public partial class NotificationHub
    {
        /// <summary>
        /// Gets the platform specific push channel
        /// </summary>
        public static string PushChannel => PlatformPushChannel;

        /// <summary>
        /// Saves the installation to the backend.  Note this should not be used in most cases.
        /// </summary>
        public static void SaveInstallation() => PlatformSaveInstallation();

        /// <summary>
        /// An event which fires when a push notification has been received.
        /// </summary>
        public static EventHandler<NotificationMessageReceivedEventArgs> NotificationMessageReceived;

        /// <summary>
        /// An event which fires when the installation has been saved to the backend.
        /// </summary>
        public static EventHandler<InstallationSavedEventArgs> InstallationSaved;

        /// <summary>
        /// An event which fires when the installation has failed to save on the backend. 
        /// </summary>
        public static EventHandler<InstallationSaveFailedEventArgs> InstallationSaveFailed;

        public static void Start(IInstallationManagementAdapter installationManagementAdapter) => PlatformInitialize(installationManagementAdapter);

        public static void Start(string connectionString, string hubName) => PlatformInitialize(connectionString, hubName);

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

        private static IInstallationManagementAdapter s_installationManagementAdapter;
        private static IInstallationEnrichmentAdapter s_enrichmentAdapter;
        public static void SetInstallationEnrichmentAdapter(IInstallationEnrichmentAdapter enrichmentAdapter)
        {
            s_enrichmentAdapter = enrichmentAdapter;
            PlatformSetEnricher();
        }
    }
}
