using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using WindowsAzure.Messaging.NotificationHubs;
using Xamarin.Essentials;

namespace NHubSampleXamarinAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Read the connections and hub name from somewhere
            var connectionString = "";
            var hubName = "";

            // Set listener for receiving messages
            NotificationHub.SetListener(new SampleNotificationListener());

            // Set an enrichment visitor
            NotificationHub.UseVisitor(new InstallationEnrichmentVisitor());

            // Set listener for installation save success and failure
            NotificationHub.SetInstallationSavedListener(new InstallationSavedListener());
            NotificationHub.SetInstallationSaveFailureListener(new InstallationSaveFailedListener());

            // Initialize with hub name and connection string
            NotificationHub.Initialize(Application, hubName, connectionString);

            // Add a tag
            NotificationHub.AddTag("target_XamarinAndroid");

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        public void AddTags()
        {
            var language = Resources.Configuration.Locales.Get(0).Language;
            var countryCode = Resources.Configuration.Locales.Get(0).Country;

            var languageTag = $"language_{language}";
            var countryCodeTag = $"country_{countryCode}";

            NotificationHub.AddTags(new [] { languageTag, countryCodeTag });
        }

        public void AddTemplate()
        {
            var language = Resources.Configuration.Locales.Get(0).Language;
            var countryCode = Resources.Configuration.Locales.Get(0).Country;

            var languageTag = $"language_{language}";
            var countryCodeTag = $"country_{countryCode}";

            var body = "{\"data\":{\"message\":\"$(message)\"}}";
            var template = new InstallationTemplate();
            template.Body = body;

            template.AddTags(new[] { languageTag, countryCodeTag });

            NotificationHub.SetTemplate("template1", template);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
