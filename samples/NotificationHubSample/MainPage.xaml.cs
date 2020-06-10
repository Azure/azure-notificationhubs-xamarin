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

            // Create Notification Hub
            NotificationHub.NotificationMessageReceived += OnNotificationMessageReceived;
            NotificationHub.Initialize(Constants.ConnectionString, Constants.HubName);

        }

        private void OnNotificationMessageReceived(object sender, NotificationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Title);
            Console.WriteLine(e.Body);
        }
    }
}
