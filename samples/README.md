# Sample applications for Azure Notification Hubs SDK for Xamarin

This directory contains the sample applications for the following:

- [Xamarin.Forms](NotificationHubSample)
- [Xamarin.iOS](bindings/NHubSampleXamariniOS)
- [Xamarin.Android](bindings/NHubSampleXamarinAndroid)

## Xamarin Forms

The [Xamarin.Forms](NotificationHubSample) uses the `Microsoft.Azure.NotificationHubs.Client` source project to create a cross platform implementation using Azure Notification Hubs.  This sample provides support for both iOS and Android.  This sample only needs code in the main NotificationHubSample project to start the SDK and listen for messages.

In the `MainPage.xaml` file, you can initialize the Azure Notification Hubs using the followingc code to listen for messages, set up installation lifecycle management, and to initialize the SDK with the Access Policy connection string and hub name.

```csharp
using Microsoft.Azure.NotificationHubs.Client;

public class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        // Listen for messages
        NotificationHub.NotificationMessageReceived += OnNotificationMessageReceived;

        // Listen for installation save updates/failures
        NotificationHub.InstallationSaved += OnInstallatedSaved;
        NotificationHub.InstallationSaveFailed += OnInstallationSaveFailed;

        // Start the SDK
        NotificationHub.Start(Constants.ConnectionString, Constants.HubName);
    }
}
```

We can then listen for messages using the `OnNotificationMessageReceived` method which has the title, body, and dictionary of the message itself.  Here, you can display the message contents.

```csharp
private void OnNotificationMessageReceived(object sender, NotificationMessageReceivedEventArgs e)
{
    // Write the message contents
    Console.WriteLine(e.Title);
    Console.WriteLine(e.Body);
}
```

We can listen for installation saved success or failures which can be helpful for diagnosing issues.

```csharp
private void OnInstallationSaveFailed(object sender, InstallationSaveFailedEventArgs e)
{
    Console.WriteLine($"Failed to save installation: {e.Exception.Message}");
}

private void OnInstallatedSaved(object sender, InstallationSavedEventArgs e)
{
    Console.WriteLine($"Installation ID: {e.Installation.InstallationId} saved successfully");
}
```

## Xamarin.iOS

We also have a [Xamarin.iOS](bindings/NHubSampleXamariniOS) sample which shows the basic usage of the [Xamarin.Azure.NotificationHubs.iOS](https://www.nuget.org/packages/Xamarin.Azure.NotificationHubs.iOS/) NuGet package which is published through the [Xamarin Components](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) repository.

To get started, register for messages in the `AppDelegate.cs` class with the following code to listen for messages, set up installation lifecycle management, and to initialize the SDK with the Access Policy connection string and hub name.

```csharp
using WindowsAzure.Messaging.NotificationHubs;

[Register("AppDelegate")]
public class AppDelegate : UIResponder, IUIApplicationDelegate
{
    [Export("application:didFinishLaunchingWithOptions:")]
    public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Listen for messages
        MSNotificationHub.SetDelegate(new NotificationDelegate());

        // Listen for installation saved success/failure
        MSNotificationHub.SetLifecycleDelegate(new InstallationLifecycleDelegate());

        // Start the Notification Hub SDK
        MSNotificationHub.Start(Constants.ConnectionString, Constants.HubName);

        return true;
    }
}
```

To listen for messages from APNS, create a class that inherits from `MSNotificationHubDelegate` which has the `DidReceivePushNotification` method.

```csharp
public class NotificationDelegate : MSNotificationHubDelegate
{
    public override void DidReceivePushNotification(MSNotificationHub notificationHub, MSNotificationHubMessage message)
    {
        // Determine whether in foreground or background
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

To listen to lifecycle management such as whether the installation save succeeded or failed on the backend, you can implement a class which inherits from the `MSInstallationLifecycleDelegate` class.

```csharp
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

## Xamarin.Android

In addition to Xamarin Forms, we also have a sample using [Xamarin.Android](bindings/NHubSampleXamarinAndroid) which shows the basic usage of the [Xamarin.Azure.NotificationHubs.Android](https://www.nuget.org/packages/Xamarin.Azure.NotificationHubs.Android/) NuGet package which is published through the [Xamarin Components](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) repository.

To get started, register for messages in the `MainActivity.cs` class with the following code to listen for messages, set up installation lifecycle management, and to initialize the SDK with the Access Policy connection string and hub name.

```csharp
using WindowsAzure.Messaging.NotificationHubs;

public class MainActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        // Set listener for receiving messages
        NotificationHub.SetListener(new SampleNotificationListener());

        // Set listener for installation save success and failure
        NotificationHub.SetInstallationSavedListener(new InstallationSavedListener());
        NotificationHub.SetInstallationSaveFailureListener(new InstallationSaveFailedListener());

        // Initialize with hub name and connection string
        NotificationHub.Start(Application, Constants.HubName, Constants.ConnectionString);
    }
}
```

To listen for messages from Firebase, create a class that inherits from `INotificationListener` interface which has the `OnPushNotificationReceived` method.

```csharp
public class SampleNotificationListener : Java.Lang.Object, INotificationListener
{
    public void OnPushNotificationReceived(Context context, INotificationMessage message)
    {
        Console.WriteLine($"Message received with title {message.Title} and body {message.Body}");
    }
}
```

To listen to lifecycle management such as whether the installation save succeeded or failed on the backend, you can implement classes which inherits from the `IInstallationAdapterListener` interface for successful saves with the `OnInstallationSaved` method which contains the current `Installation`.

```csharp
// Listen for installation saved success
public class InstallationSavedListener : Java.Lang.Object, IInstallationAdapterListener
{
    public void OnInstallationSaved(Installation installation)
    {
        Console.WriteLine($"Installation successfully saved with Installation ID: {installation.InstallationId}");
    }
}
```

We can then listen for installation saved failures using the `IInstallationAdapterErrorListener` interface which has the `OnInstallationSaveError` method with the exception.

```csharp
public class InstallationSaveFailedListener : Java.Lang.Object, IInstallationAdapterErrorListener
{
    public void OnInstallationSaveError(Java.Lang.Exception javaException)
    {
        var exception = Throwable.FromException(javaException);
        Console.WriteLine($"Save installation failed with exception: {exception.Message}");
    }
}
```
