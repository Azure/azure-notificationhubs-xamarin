using System;
using Foundation;
using UIKit;
using WindowsAzure.Messaging.NotificationHubs;

namespace NHubSampleXamariniOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate
    {

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Get settings from the DevSettings plist
            var path = NSBundle.MainBundle.PathForResource("DevSettings", "plist");
            var configValues = NSDictionary.FromFile(path);

            // Read from the plist
            var connectionString = configValues.ObjectForKey(new NSString("ConnectionString"));
            var hubName = configValues.ObjectForKey(new NSString("HubName"));

            if (connectionString == null || hubName == null)
            {
                Console.WriteLine("Connection String and Hub Name missing");
                return false;
            }

            // Set a listener for messages
            MSNotificationHub.SetDelegate(new NotificationDelegate());

            // Set a listener for lifecycle management
            MSNotificationHub.SetLifecycleDelegate(new InstallationLifecycleDelegate());

            // Start the SDK
            MSNotificationHub.Start(connectionString.ToString(), hubName.ToString());

            // Add some tags
            AddTags();

            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            return true;
        }

        public void AddTags()
        {
            var language = NSBundle.MainBundle.PreferredLocalizations[0];
            var countryCode = NSLocale.CurrentLocale.CountryCode;

            var languageTag = $"language_{language}";
            var countryCodeTag = $"country_{countryCode}";

            MSNotificationHub.AddTag(languageTag);
            MSNotificationHub.AddTag(countryCodeTag);
        }

        public void AddTemplate()
        {
            var language = NSBundle.MainBundle.PreferredLocalizations[0];
            var countryCode = NSLocale.CurrentLocale.CountryCode;

            var languageTag = $"language_{language}";
            var countryCodeTag = $"country_{countryCode}";

            var body = "{\"aps\": {\"alert\": \"$(message)\"}}";
            var template = new MSInstallationTemplate();
            template.Body = body;
            template.AddTag(languageTag);
            template.AddTag(countryCodeTag);

            MSNotificationHub.SetTemplate(template, key: "template1");
        }

        // UISceneSession Lifecycle

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}

