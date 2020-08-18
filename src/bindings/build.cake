var TARGET = Argument ("t", Argument ("target", "ci"));

var IOS_VERSION = "3.1.0";
var IOS_NUGET_VERSION = "3.1.0";
var IOS_URL = $"https://github.com/Azure/azure-notificationhubs-ios/releases/download/{IOS_VERSION}/WindowsAzureMessaging-SDK-Apple-{IOS_VERSION}.zip";

var ANDROID_VERSION = "1.0.1";
var ANDROID_NUGET_VERSION = "1.0.1";
var ANDROID_URL = string.Format ("https://dl.bintray.com/microsoftazuremobile/SDK/com/microsoft/azure/notification-hubs-android-sdk/{0}/notification-hubs-android-sdk-{0}.aar", ANDROID_VERSION);

Task("libs-ios")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("externals-ios")
	.Does (() =>
{
	MSBuild("./iOS/Xamarin.Azure.NotificationHubs.iOS/Xamarin.Azure.NotificationHubs.iOS.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Rebuild");
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/libs-ios.binlog"
		};
	});
});

Task("libs-android")
	.IsDependentOn("externals-android")
	.Does (() =>
{
	MSBuild("./Android/Xamarin.Azure.NotificationHubs.Android/Xamarin.Azure.NotificationHubs.Android.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Rebuild");
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/libs-android.binlog"
		};
	});
});

Task("nuget-ios")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("libs-ios")
	.Does (() =>
{
	MSBuild ("./iOS/Xamarin.Azure.NotificationHubs.iOS/Xamarin.Azure.NotificationHubs.iOS.sln", c => {
		c.Configuration = "Release";
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/nuget-ios.binlog"
		};
	});
});


Task("nuget-android")
	.IsDependentOn("libs-android")
	.Does (() =>
{
	MSBuild ("./Android/Xamarin.Azure.NotificationHubs.Android/Xamarin.Azure.NotificationHubs.Android.sln", c => {
		c.Configuration = "Release";
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/nuget-android.binlog"
		};
	});
});

Task("ci")
	.IsDependentOn("nuget");


Task ("externals-ios")
	.WithCriteria(IsRunningOnUnix())
	.WithCriteria (!FileExists ("./iOS/externals/sdk.zip"))
	.Does (() => 
{
	EnsureDirectoryExists ("./iOS/externals");

	DownloadFile (IOS_URL, "./iOS/externals/sdk.zip");

	Unzip ("./iOS/externals/sdk.zip", "./iOS/externals");
	
	CreateDirectory("./iOS/externals/iOS");
	CreateDirectory("./iOS/externals/macOS");
	CreateDirectory("./iOS/externals/tvOS");

	CopyFile(
		"./iOS/externals/WindowsAzureMessaging-SDK-Apple/iOS/WindowsAzureMessaging.framework/WindowsAzureMessaging",
		"./iOS/externals/iOS/WindowsAzureMessaging.a");

	CopyFile(
		"./iOS/externals/WindowsAzureMessaging-SDK-Apple/macOS/WindowsAzureMessaging.framework/WindowsAzureMessaging",
		"./iOS/externals/macOS/WindowsAzureMessaging.a");

	CopyFile(
		"./iOS/externals/WindowsAzureMessaging-SDK-Apple/tvOS/WindowsAzureMessaging.framework/WindowsAzureMessaging",
		"./iOS/externals/tvOS/WindowsAzureMessaging.a");

	XmlPoke("./iOS/Xamarin.Azure.NotificationHubs.iOS/Xamarin.Azure.NotificationHubs.iOS.csproj", "/Project/PropertyGroup/PackageVersion", IOS_NUGET_VERSION);
});

Task ("externals-android")
	.WithCriteria (!FileExists ("./externals/Android/notificationhubs.aar"))
	.Does (() => 
{
	EnsureDirectoryExists ("./Android/externals");

	Information($"Downloading from {ANDROID_URL}");
	DownloadFile (ANDROID_URL, "./Android/externals/notificationhubs.aar");
	
	XmlPoke("./Android/Xamarin.Azure.NotificationHubs.Android/Xamarin.Azure.NotificationHubs.Android.csproj", "/Project/PropertyGroup/PackageVersion", ANDROID_NUGET_VERSION);
});

Task ("externals")
	.IsDependentOn ("externals-ios")
	.IsDependentOn ("externals-android");

Task ("nuget")
	.IsDependentOn ("nuget-ios")
	.IsDependentOn ("nuget-android");

Task ("libs")
	.IsDependentOn ("libs-ios")
	.IsDependentOn ("libs-android");

Task ("clean")
	.Does (() => 
{
	if (DirectoryExists ("./Android/externals"))
		DeleteDirectory ("./Android/externals", true);

	if (DirectoryExists ("./iOS/externals"))
		DeleteDirectory ("./iOS/externals", true);
});

RunTarget (TARGET);
