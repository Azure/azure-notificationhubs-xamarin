using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    /// <summary>
    /// This represents an adapter to allows the user to save the installation on the back-end and report back either success or failure.
    /// </summary>
    public interface IInstallationManagementAdapter
    {
        /// <summary>
        /// Saves an installation to a back-end.  To finish the call, one must invoke onSuccess for success or onError for failure.
        /// </summary>
        /// <param name="installation">The installation to save the back-end.</param>
        /// <param name="onSuccess">The success action to invoke upon successful save.</param>
        /// <param name="onError">The failure action to invoke when a save has failed.</param>
        void SaveInstallation(Installation installation, Action<Installation> onSuccess, Action<Exception> onError);
    }
}
