using System;

namespace Microsoft.Azure.NotificationHubs.Client
{
    public interface IInstallationManagementAdapter
    {
        void SaveInstallation(Installation installation, Action<Installation> onSuccess, Action<Exception> onError);
    }
}
