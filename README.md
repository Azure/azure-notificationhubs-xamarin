# Azure Notification Hubs for Xamarin

This repository contains the Azure Notification Hubs sample library and sample application for Xamarin Forms, Xamarin for Apple (Xamarin.iOS, Xamarin.TVOS, Xamarin.Mac) and Xamarin.Android.  This sample project creates a unified wrapper over the [AzureMessaging Xamarin Components](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) for both iOS and Android with a Forms app as well as native applications.

This repository is set into sections:
- [Xamarin Forms](#getting-started-with-xamarinforms)
- [Xamarin Apple](#getting-started-with-xamarin-for-apple)
- [Xamarin.Android](#getting-started-with-xamarinandroid)

## Getting Started with Xamarin.Forms

This project contains `Microsoft.Azure.NotificationHubs.Client`, which when added to your project, provides a consistent experience across Xamarin Android and Xamarin for Apple.  Either download or clone the repository and either add the source from the project to your own solution, or create a NuGet package from the source and add from a local NuGet source.

Once you have referenced either the source code or local NuGet, you can add the project to your main Xamarin Forms project as well as the Android and Apple specific projects.

Initializating the Azure Notification Hubs sample can be done by importing the namespace from the sample.

```csharp
using Microsoft.Azure.NotificationHubs.Client;
```

And then in the start of your application, for example in your MainPage.xaml, you can initialize the `NotificationHub` class with the hub name and access policy connection string, referencing them for example from a constants file or some other configuration settings.

```csharp
// Start the API
NotificationHub.Start(Constants.ConnectionString, Constants.HubName);
```

You can then listen for push notifications in your application by adding an event handler to the `NotificationMessageReceived` event.

```csharp
// Add an event handler
NotificationHub.NotificationMessageReceived += OnNotificationMessageReceived;

// The handler
private void OnNotificationMessageReceived(object sender, NotificationMessageReceivedEventArgs e)
{
    // Do something with the message
    Console.WriteLine($"Title: {e.Title}");
    Console.WriteLine($"Body: {e.Body}");
}
```

The API automatically handles using the Installation API for you once you have called the `NotificationHub.Start` method.  You may want indiciations that either the installation was successfully saved or failed to save, and to cover those scenarios, we have the `InstallationSaved` and `InstallationSaveFailed` events that you can add handlers to.  The `InstallationSaved` event will give you back the `Installation` that was saved, and the `InstallationSaveFailed` will give you back an `Exception` from the process.

```csharp
// Add handlers for installation managements
NotificationHub.InstallationSaved += OnInstallatedSaved;
NotificationHub.InstallationSaveFailed += OnInstallationSaveFailed;

private void OnInstallatedSaved(object sender, InstallationSavedEventArgs e)
{
    Console.WriteLine($"Installation ID: {e.Installation.InstallationId} saved successfully");
}

private void OnInstallationSaveFailed(object sender, InstallationSaveFailedEventArgs e)
{
    Console.WriteLine($"Failed to save installation: {e.Exception.Message}");
}
```

In the case of failure, you can try the process of saving the installation again by calling the `NotificationHub.SaveInstallation()` method, but this should be only called in case the installation process has failed.

### Tag Management

One of the ways to target a device or set of devices is through the use of [tags](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-tags-segment-push-message#tags), where you can target a specific tag, or a tag expression.  The SDK handles this through top level methods that allow you to add, clear, remove and get all tags for the current installation.  In this example, we can add some recommended tags such as the app language preference, and device country code.

```csharp
var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
var country = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;

var languageTag = $"language_{langauge}";
var countryTag = $"country_{country}";

NotificationHub.AddTags(new [] { languageTag, countryTag });
```

### Template Management

With [Azure Notification Hub Templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-templates-cross-platform-push-messages), you can enable a client application to specify the exact format of the notifications it wants to receive.  This is useful when you want to create a more personalized notification, with string replacement to fill the values.  The Installation API [allows multiple templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#templates) for each installation which gives you greater power to target your users with rich messages.

For example, we can create a template with a body, some headers, and some tags.

```csharp
var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
var country = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;

var languageTag = $"language_{langauge}";
var countryTag = $"country_{country}";

var body = "{\"aps\": {\"alert\": \"$(message)\"}}";
var template = new InstallationTemplate();
template.Body = body;
template.Tags = new List<string> { languageTag, countryCodeTag };

NotificationHub.SetTemplate("template1", template);
```

### Enriching Installations

The SDK will update the installation on the device any time you change its properties such as adding a tag or adding an installation template. Before the installation is sent to the backend, you can intercept this installation to modify anything before it goes to the backend, for example, if you wish to add or modify tags. This is implemented in the `IInstallationEnrichmentAdapter` interface with a single method of `EnrichInstallation`.

```csharp
NotificationHub.SetInstallationEnrichmentAdapter(new InstallationEnrichmentAdapter());

public class InstallationEnrichmentAdapter : IInstallationEnrichmentAdapter
{
    public override void EnrichInstallation(Installation installation)
    {
        installation.Tags.Add("platform_XamarinForms");
    }
}
```

### Saving Installations to a Custom Backend

The Azure Notification Hubs SDK will save the installation to our backend by default. If, however, you wish to skip our backend and store it on your backend, we support that through the `IInstallationManagementAdapter` interface. This has a method to save the installation `SaveInstallation`, passing in the installation, and then a completion for success, or for errors.  Instead of starting the API with the `Start` method with the hub name and connection string, we start with our installation adapter.

```csharp
// Set the installation management delegate
Notification.Start(new InstallationManagementAdapter()); 

public class InstallationManagementDelegate : IInstallationManagementAdapter
{
    void SaveInstallation(Installation installation, Action<Installation> onSuccess, Action<Exception> onError);
    {
        // Save the installation to your own backend
        // Finish with a completion with error if one occurred, else null
        onSuccess(installation);
    }
}
```

### Extending the Xamarin Forms Example

This sample is meant to be a starting point for wrapping the Azure Notification Hubs Xamarin Components.  Extending the capabilities for each platform, Apple and Android, can be done in files that end with `.ios.cs` or `.android.cs` depending on the platform.  Extending shared pieces of code can be added by adding files ending with the `.shared.cs` extension.  If you need platform specific code for your Xamarin Forms iOS or Android projects, you can reference the `NotificationHub` class which will reference the appropriate platform.

## Getting Started with Xamarin for Apple

The Azure Notification Hubs for Xamarin for Apple (Xamarin.iOS, Xamarin.TVOS, Xamarin.Mac) is supported as part of the [Xamarin Components](https://github.com/xamarin/XamarinComponents) repository in the [AzureMessaging](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) folder.  This provides the `Xamarin.Azure.NotificationHubs.iOS` NuGet package which can be added to your Xamarin Apple project.

Initializating the Azure Notification Hubs for Xamarin Apple can be done by importing the `WindowsAzure.Messaging.NotificationHubs` namespace from the package.  **Note there are other classes under the `WindowsAzure.Messaging` that are still available and supported such as `SBNotificationHub` but are discouraged.**

```csharp
using WindowsAzure.Messaging.NotificationHubs;
```

And then in the start of your application, for example in your AppDelegate.cs, you can initialize the `MSNotificationHub` class with the hub name and access policy connection string, referencing them for example from a constants file or some other configuration settings.

```csharp
// Start the API
MSNotificationHub.Start(Constants.ConnectionString, Constants.HubName);
```

You can then listen for push notifications in your application by extending the `MSNotificationHubDelegate` class and implementing the abstract `DidReceivePushNotification` method.

```csharp
// Set the delegate for receiving messages
MSNotificationHub.SetDelegate(new NotificationDelegate());

// The notification delegate implementation
public class NotificationDelegate : MSNotificationHubDelegate
{
    public override void DidReceivePushNotification(MSNotificationHub notificationHub, MSNotificationHubMessage message)
    {
        if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background)
        {
            Console.WriteLine($"Message received in the background with title {message.Title} and body {message.Body}");
        }
        else
        {
            Console.WriteLine($"Message received in the foreground with title {message.Title} and body {message.Body}");
        }
    }
}
```

### Tag Management

One of the ways to target a device or set of devices is through the use of [tags](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-tags-segment-push-message#tags), where you can target a specific tag, or a tag expression.  The SDK handles this through top level methods that allow you to add, clear, remove and get all tags for the current installation.  In this example, we can add some recommended tags such as the app language preference, and device country code.

```csharp
var language = NSBundle.MainBundle.PreferredLocalizations[0];
var countryCode = NSLocale.CurrentLocale.CountryCode;

var languageTag = $"language_{language}";
var countryCodeTag = $"country_{countryCode}";

MSNotificationHub.AddTag(languageTag);
MSNotificationHub.AddTag(countryCodeTag);
```

### Template Management

With [Azure Notification Hub Templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-templates-cross-platform-push-messages), you can enable a client application to specify the exact format of the notifications it wants to receive.  This is useful when you want to create a more personalized notification, with string replacement to fill the values.  The Installation API [allows multiple templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#templates) for each installation which gives you greater power to target your users with rich messages.

For example, we can create a template with a body, some headers, and some tags.

```csharp
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
```

### Intercepting Installation Management

The SDK will handle saving the installation for you, however, we provide hooks where you can intercept both the successful installation or any failure through the `MSInstallationLifecycleDelegate` abstract class.  This has two methods, `DidSaveInstallation` for successful saves, and `DidFailToSaveInstallation` for any failures.  We can implement this to have our own logging for example.  

```csharp
// Set a listener for lifecycle management
MSNotificationHub.SetLifecycleDelegate(new InstallationLifecycleDelegate());

public class InstallationLifecycleDelegate : MSInstallationLifecycleDelegate
{
    public override void DidFailToSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation, NSError error)
    {
        Console.WriteLine($"Save installation failed with exception: {error.LocalizedDescription}");
    }

    public override void DidSaveInstallation(MSNotificationHub notificationHub, MSInstallation installation)
    {
        Console.WriteLine($"Installation successfully saved with Installation ID: {installation.InstallationId}");
    }
}
```

### Enriching Installations

The SDK will update the installation on the device any time you change its properties such as adding a tag or adding an installation template. Before the installation is sent to the backend, you can intercept this installation to modify anything before it goes to the backend, for example, if you wish to add or modify tags. This is implemented in the `MSInstallationEnrichmentDelegate` abstract base class with a single method of `WillEnrichInstallation`.

```csharp
// Set a dele3gate for enriching installations
MSNotificationHub.SetEnrichmentDelegate(new InstallationEnrichmentDelegate());

public class InstallationEnrichmentDelegate : MSInstallationEnrichmentDelegate
{
    public override void WillEnrichInstallation(MSNotificationHub notificationHub, MSInstallation installation)
    {
        installation.AddTag("platform_XamariniOS");
    }
}
```

### Saving Installations to a Custom Backend

The Azure Notification Hubs SDK will save the installation to our backend by default. If, however, you wish to skip our backend and store it on your backend, we support that through the `MSInstallationManagementDelegate` protocol. This has a method to save the installation `WillUpsertInstallation`, passing in the installation, and then a completion handler is called with either an error if unsuccessful, or nil if successful.  Instead of starting the API with the `Start` method, we use the `StartWithInstallationManagement` which sets the installation manager.

```csharp
// Set the installation management delegate
MSNotificationHub.StartWithInstallationManagement(new InstallationManagementDelegate()); 

public class InstallationManagementDelegate : MSInstallationManagementDelegate
{
    public override void WillUpsertInstallation(MSNotificationHub notificationHub, MSInstallation installation, NullableCompletionHandler completionHandler)
    {
        // Save the installation to your own backend
        // Finish with a completion with error if one occurred, else null
        completionHandler(null);
    }
}
```

### Disabling Automatic Swizzling

By default, the SDK will swizzle methods to automatically intercept calls to `UIApplicationDelegate`/`NSApplicationDelegate` for calls to registering and intercepting push notifications, as well as `UNUserNotificationCenterDelegate` methods.  

#### Disabling UIApplicationDelegate/NSApplicationDelegate

1. Open the project's Info.plist
2. Add the `NHAppDelegateForwarderEnabled` key and set the value to 0.  This disables the UIApplicationDelegate/NSApplicationDelegate auto-forwarding to MSNotificaitonHub.
3. Implement the `MSApplicationDelegate`/`NSApplicationDelegate` methods for push notifications.

    Implement the `RegisteredForRemoteNotifications` callback and the `FailedToRegisterForRemoteNotifications` callback in your AppDelegate to register for Push notifications.

    ```csharp
    public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        MSNotificationHub.DidRegisterForRemoteNotifications(deviceToken);
    }

    public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        MSNotificationHub.DidFailToRegisterForRemoteNotifications(error);
    }
    ```

4. Implement the callback to receive push notifications

    Override the `DidReceiveRemoteNotification` method to forward push notifications to MSNotificationHub.

    ```csharp
    public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
    {

        // Forward to MSNotificationHub
        MSNotificationHub.DidReceiveRemoteNotification(userInfo);

        // Complete handling the notification
        completionHandler(UIBackgroundFetchResult.NoData);
    }
    ```

#### Disabling UNUserNotificationCenterDelegate

1. Open the project's Info.plist
2. Add the `NHUserNotificationCenterDelegateForwarderEnabled` key and set the value to 0.  This disables the UNUserNotificationCenterDelegate auto-forwarding to MSNotificaitonHub.
3. Implement UNUserNotificationCenterDelegate callbacks and pass the notification's payload to `MSNotificationHub`.

    ```csharp
    public override void WillPresentNotification (UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        //...

        // Pass the notification payload to MSNotificationHub
        MSNotificationHub.DidReceiveRemoteNotification(notification.Request.Content.UserInfo);

        // Complete handling the notification
        completionHandler(UNNotificationPresentationOptions.None);
    }

    public override void DidReceiveNotificationResponse (UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler) 
    {
        //...

        // Pass the notification payload to MSNotificationHub
        MSNotificationHub.DidReceiveRemoteNotification(response.Notification.Request.Content.UserInfo);

        // Complete handling the notification
        completionHandler();
    }
    ```

## Getting Started with Xamarin.Android

The Azure Notification Hubs for Xamarin.Android is supported as part of the [Xamarin Components](https://github.com/xamarin/XamarinComponents) repository in the [AzureMessaging](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) folder.  This provides the `Xamarin.Azure.NotificationHubs.Android` NuGet package which can be added to your Xamarin.Android project.

Initializating the Azure Notification Hubs for Xamarin.Android can be done by importing the `WindowsAzure.Messaging.NotificationHubs` namespace from the package.  **Note there are other classes under the `WindowsAzure.Messaging` that are still available and supported such as the other `NotificationHub` class, but are discouraged.**

```csharp
using WindowsAzure.Messaging.NotificationHubs;
```

And then in the start of your application, for example in your MainActivity.cs, you can initialize the `NotificationHub` class with the Android `Application`, hub name and access policy connection string, referencing them for example from a constants file or some other configuration settings.

```csharp
// Start the API
NotificationHub.Start(this.Application, Constants.HubName, Constants.ConnectionString);
```

You can then listen for push notifications in your application by implementing the `INotificationListener` interface and implementing the `OnPushNotificationReceived` method.

```csharp
// Set the delegate for receiving messages
NotificationHub.SetListener(new SampleNotificationListener());

// The notification listener implementation
public class SampleNotificationListener : Java.Lang.Object, INotificationListener
{
    public void OnPushNotificationReceived(Context context, INotificationMessage message)
    {
        Console.WriteLine($"Message received with title {message.Title} and body {message.Body}");
    }
}
```

### Tag Management

One of the ways to target a device or set of devices is through the use of [tags](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-tags-segment-push-message#tags), where you can target a specific tag, or a tag expression.  The SDK handles this through top level methods that allow you to add, clear, remove and get all tags for the current installation.  In this example, we can add some recommended tags such as the app language preference, and device country code.

```csharp
var language = Resources.Configuration.Locales.Get(0).Language;
var countryCode = Resources.Configuration.Locales.Get(0).Country;

var languageTag = $"language_{language}";
var countryCodeTag = $"country_{countryCode}";

NotificationHub.AddTags(new [] { languageTag, countryCodeTag });
```

### Template Management

With [Azure Notification Hub Templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-templates-cross-platform-push-messages), you can enable a client application to specify the exact format of the notifications it wants to receive.  This is useful when you want to create a more personalized notification, with string replacement to fill the values.  The Installation API [allows multiple templates](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#templates) for each installation which gives you greater power to target your users with rich messages.

For example, we can create a template with a body, some headers, and some tags.

```csharp
var language = Resources.Configuration.Locales.Get(0).Language;
var countryCode = Resources.Configuration.Locales.Get(0).Country;

var languageTag = $"language_{language}";
var countryCodeTag = $"country_{countryCode}";

var body = "{\"data\":{\"message\":\"$(message)\"}}";
var template = new InstallationTemplate();
template.Body = body;

template.AddTags(new[] { languageTag, countryCodeTag });

NotificationHub.SetTemplate("template1", template);
```

### Intercepting Installation Management

The SDK will handle saving the installation for you, however, we provide hooks where you can intercept both the successful installation or any failure through the `IInstallationAdapterListener` interface for successful saves and the `IInstallationAdapterErrorListener` interface for any errors. 

```csharp
// Set listener for installation save success and failure
NotificationHub.SetInstallationSavedListener(new InstallationSavedListener());
NotificationHub.SetInstallationSaveFailureListener(new InstallationSaveFailedListener());

public class InstallationSavedListener : Java.Lang.Object, IInstallationAdapterListener
{
    public void OnInstallationSaved(Installation installation)
    {
        Console.WriteLine($"Installation successfully saved with Installation ID: {installation.InstallationId}");
    }
}

public class InstallationSaveFailedListener : Java.Lang.Object, IInstallationAdapterErrorListener
{
    public void OnInstallationSaveError(Java.Lang.Exception javaException)
    {
        var exception = Throwable.FromException(javaException);
        Console.WriteLine($"Save installation failed with exception: {exception.Message}");
    }
}
```

### Enriching Installations

The SDK will update the installation on the device any time you change its properties such as adding a tag or adding an installation template. Before the installation is sent to the backend, you can intercept this installation to modify anything before it goes to the backend, for example, if you wish to add or modify tags. This is implemented using the Visitor Pattern through the `IInstallationVisitor` interface.

```csharp
// Set an enrichment visitor
NotificationHub.UseVisitor(new InstallationEnrichmentVisitor());

public class InstallationEnrichmentVisitor : Java.Lang.Object, IInstallationVisitor
{
    public void VisitInstallation(Installation installation)
    {
        // Add a sample tag
        installation.AddTag("platform_XamarinAndroid");
    }
}
```

The Azure Notification Hubs SDK will save the installation to our backend by default. If, however, you wish to skip our backend and store it on your backend, we support that through the `IInstallationAdapter` interface. This has a method to save the installation `SaveInstallation`, passing in the installation, and two listeners, one for successful saves with the `IInstallationAdapterListener` and `IInstallationAdapterErrorListener` for any failures.  Instead of starting the API with the `Start` method with the hub name and connection string, we use the `IInstallationAdapter` overload.

```csharp
// Set the installation management delegate
NotificationHub.Start(Application, new SampleInstallationAdapter()); 

public class SampleInstallationAdapter : Java.Lang.Object, IInstallationAdapter
{
    public void SaveInstallation(Installation installation,
        IInstallationAdapterListener installationAdapterListener,
        IInstallationAdapterErrorListener installationAdapterErrorListener)
    {
        // Save to your own backend

        // Call if successfully saved
        installationAdapterListener.OnInstallationSaved(installation);
    }
}
```

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to
agree to a Contributor License Agreement (CLA) declaring that you have the right to,
and actually do, grant us the rights to use your contribution. For details, visit
https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need
to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the
instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License

.NET (including the runtime repo) is licensed under the [MIT](LICENSE) license.
