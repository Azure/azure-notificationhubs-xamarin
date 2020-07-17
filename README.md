# Azure Notification Hubs Sample for Xamarin Forms

This repository contains the Azure Notification Hubs sample library and sample application for Xamarin Forms.  This sample project creates a unified wrapper over the [AzureMessaging Xamarin Components](https://github.com/xamarin/XamarinComponents/tree/master/XPlat/AzureMessaging) for both iOS and Android. 

## Getting Started

This project contains `Microsoft.Azure.NotificationHubs.Client`, which when added to your project, provides a consistent experience across Xamarin Android and Xamarin iOS.  Either download or clone the repository and either add the source from the project to your own solution, or create a NuGet package from the source and add from a local NuGet source.

Once you have referenced either the source code or local NuGet, you can add the project to your main Xamarin Forms project as well as the Android and iOS specific projects.

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

## Extending the Sample

This sample is meant to be a starting point for wrapping the Azure Notification Hubs Xamarin Components.  Extending the capabilities for each platform, iOS and Android, can be done in files that end with `.ios.cs` or `.android.cs` depending on the platform.  Extending shared pieces of code can be added by adding files ending with the `.shared.cs` extension.  If you need platform specific code for your Xamarin Forms iOS or Android projects, you can reference the `NotificationHub` class which will reference the appropriate platform.

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
