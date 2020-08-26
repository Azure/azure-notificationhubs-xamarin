using System;
using System.ComponentModel;
using Xamarin.Forms;
using Microsoft.Azure.NotificationHubs.Client;

namespace NotificationHubSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            // Add a handler for receiving push notifications
            NotificationHub.NotificationMessageReceived += OnNotificationMessageReceived;

            // Add handlers for save and save failed for installations
            NotificationHub.InstallationSaved += OnInstallatedSaved;
            NotificationHub.InstallationSaveFailed += OnInstallationSaveFailed;

            // Create Notification Hub
            NotificationHub.Start(Constants.ConnectionString, Constants.HubName);

            // Add a tag
            NotificationHub.AddTag("platform_XamarinForms");
        }

        private void OnInstallationSaveFailed(object sender, InstallationSaveFailedEventArgs e)
        {
            Console.WriteLine($"Failed to save installation: {e.Exception.Message}");
        }

        private void OnInstallatedSaved(object sender, InstallationSavedEventArgs e)
        {
            Console.WriteLine($"Installation ID: {e.Installation.InstallationId} saved successfully");
        }

        private void OnNotificationMessageReceived(object sender, NotificationMessageReceivedEventArgs e)
        {
            // Write the message contents
            Console.WriteLine(e.Title);
            Console.WriteLine(e.Body);
        }
    }
}
