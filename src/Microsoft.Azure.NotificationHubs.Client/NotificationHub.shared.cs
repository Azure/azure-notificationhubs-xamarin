using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// Handles the interactions with Azure Notification Hubs
    /// </summary>
    public partial class NotificationHub
    {
        /// <summary>
        /// Gets or set the user ID.
        /// </summary>
        public static string UserId
        {
            get => PlatformGetUserId();
            set => PlatformSetUserId(value);
        }

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

        /// <summary>
        /// Starts the Notification Hub with a connection string from the Access Policy and the Hub Name.
        /// </summary>
        /// <param name="connectionString">The connection string from the Access Policy.</param>
        /// <param name="hubName">The Azure Notification Hub name.</param>
        public static void Start(string connectionString, string hubName) => PlatformInitialize(connectionString, hubName);

        /// <summary>
        /// Starts the Notification Hub with a custom backend for saving an installation.
        /// </summary>
        /// <param name="installationManagementAdapter">The installation management adapter to save an installation to a custom backend.</param>
        public static void Start(IInstallationManagementAdapter installationManagementAdapter) => PlatformInitialize(installationManagementAdapter);


        #region Tags

        /// <summary>
        /// Adds a tag to the installation.
        /// </summary>
        /// <param name="tag">The tag to add to the installation.</param>
        /// <returns>Returns true if added, otherwise false.</returns>
        public static bool AddTag(string tag) => PlatformAddTag(tag);

        /// <summary>
        /// Adds an array of tags to the installation.
        /// </summary>
        /// <param name="tags">The tags to add to the installation.</param>
        /// <returns>Returns true if added, otherwise false.</returns>
        public static bool AddTags(string[] tags) => PlatformAddTags(tags);

        /// <summary>
        /// Clears the tags from the installation.
        /// </summary>
        public static void ClearTags() => PlatformClearTags();

        /// <summary>
        /// Removes a tag from the installation.
        /// </summary>
        /// <param name="tag">The tag to remove from the installation.</param>
        /// <returns>Returns true if removed, otherwise false.</returns>
        public static bool RemoveTag(string tag) => PlatformRemoveTag(tag);

        /// <summary>
        /// Removes an array of tags from the installation.
        /// </summary>
        /// <param name="tags">The tags to remove from the installation.</param>
        /// <returns>Returns true if removed, false otherwise.</returns>
        public static bool RemoveTags(string[] tags) => PlatformRemoveTags(tags);

        /// <summary>
        /// Gets an array of the tags on the installation.
        /// </summary>
        /// <returns>The tags on the installation.</returns>
        public static string[] GetTags() => PlatformGetTags();

        #endregion

        #region Templates

        /// <summary>
        /// Sets the Installation Template for the given name.
        /// </summary>
        /// <param name="name">The name for the installation template.</param>
        /// <param name="template">The installation template to save to the installation.</param>
        public static void SetTemplate(string name, InstallationTemplate template) => PlatformSetTemplate(name, template);

        /// <summary>
        /// Removes the Installation Template from the installation by the name.
        /// </summary>
        /// <param name="name">The name of the template to remove.</param>
        public static void RemoveTemplate(string name) => PlatformRemoveTemplate(name);

        /// <summary>
        /// Gets the Installation Template by the name.
        /// </summary>
        /// <param name="name">The name of the Installation Template.</param>
        /// <returns>The Installation Template that was stored with the given name.</returns>
        public static InstallationTemplate GetTemplate(string name) => PlatformGetTemplate(name);

        #endregion

        private static IInstallationEnrichmentAdapter s_enrichmentAdapter;

        /// <summary>
        /// Sets the installation enrichment adapter which allows to set properties on the installation before saving.
        /// </summary>
        /// <param name="enrichmentAdapter">The enrichment adapter to use to enrich the installation.</param>
        public static void SetInstallationEnrichmentAdapter(IInstallationEnrichmentAdapter enrichmentAdapter)
        {
            s_enrichmentAdapter = enrichmentAdapter;
            PlatformSetEnricher();
        }
    }
}
