# Windows Azure Mobile Services

With Windows Azure Mobile Services you can add a scalable backend to your connected client applications in minutes. To learn more, visit our [Developer Center](http://www.windowsazure.com/en-us/develop/mobile).

## Getting Started

If you are new to Mobile Services, you can get started by following our tutorials for connecting your Mobile Services cloud backend to [Windows Store apps](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started/) and [iOS apps](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started-ios/).

## Download Source Code

To get the source code of our SDKs and samples via **git** just type:

    git clone https://github.com/xamarin/azure-mobile-services.git
    cd ./azure-mobile-services/

## Xamarin and Azure: MonoTouch for iOS and Mono for Android Client SDK

Our client SDK makes it incredibly easy to use Mobile Services from your MonoTouch for iOS and Mono for Android applications. You can download the source code using the instructions above and then you will find the SDKs under ```/azure-mobile-services/sdk/xamarin/ios``` and ```/azure-mobile-services/sdk/xamarin/android```.

### Prerequisities

The SDK requires MonoTouch for iOS or Mono for Android.

### Limitations

For MonoTouch, in your "Additional mtouch arguments" section, ```--linkskip=mscorlib``` may be required if SDK linking is enabled.
Additionally, the SDK does not provide support for push notifications, this is currently limited to Windows 8.

### Running the Tests

The MonoTouch SDK has a suite of unit tests. 

1. Open the ```/azure-mobile-services/sdk/ios/Microsoft.Azure.Zumo.iOS.sln``` solution file in MonoDevelop.
2. Right click on the ```ZumoTests``` project in the solution explorer and select ```Set as StartUp Project```.
3. Start the application
4. A MonoTouch application will appear, press ```Run Everything```

### Sample Application: Todo

Todo is a simple todo list application that demonstrates some features of Windows Azure Mobile Services. You can find todo under ```/azure-mobile-services/samples/todo```.

### Contribute Code or Provide Feedback

If you would like to become an active contributor to this project please follow the instructions provided in [Windows Azure Projects Contribution Guidelines](http://windowsazure.github.com/guidelines.html). Fixes or features not specific to iOS or Android should go to the [original repository](https://github.com/WindowsAzure/azure-mobile-services) so that everyone can benefit.

If you encounter any bugs with the library please file an issue in the [Issues](https://github.com/xamarin/azure-mobile-services/issues) section of the project.

## Windows Client SDK

The Windows SDK makes it incredibly easy to use Mobile Services from your Windows Store applications. You can [download the SDK](http://go.microsoft.com/fwlink/?LinkId=257545&clcid=0x409) directly or you can download the source code using the instructions above and then you will find the Windows Client SDK under ```/azure-mobile-services/sdk/windows```.

## iOS Client SDK
Note: This iOS client is released as a preview. It is currently under development.

Add a cloud backend to your iOS application in minutes with our iOS client SDK. You can [download the iOS SDK](https://go.microsoft.com/fwLink/?LinkID=266533&clcid=0x409) directly or you can download the source code using the instructions above and then you will find the SDK under ```azure-mobile-services/sdk/iOS```. As always we’re excited to get your feedback on this early version of the library on [our issues page](https://github.com/WindowsAzure/azure-mobile-services/issues), and welcome your [contributions](http://windowsazure.github.com/guidelines.html). 

### Prerequisities

The SDK requires Windows 8 RTM and Visual Studio 2012 RTM.

### Running the Tests

The Windows SDK has a suite of unit tests but the process for running these tests might be unfamiliar. 

1. Open the ```/azure-mobile-services/sdk/windows/win8sdk.sln``` solution file in Visual Studio 2012.
2. Right click on the ```Microsoft.Azure.Zumo.Windows.CSharp.Test``` project in the solution explorer and select ```Set as StartUp Project```.
3. Press F5
4. A Windows Store application will appear with a prompt for a Runtime Uri and Tags. You can safely ignore this prompt and just click the Start button.
5. The test suite will run and display the results.

### Building and Referencing the SDK

When you build the solution the output is written to the  ```/azure-mobile-services/sdk/windows/bin``` folder. To reference the SDK from a C# Windows Store application, use the dll located at
 ```/azure-mobile-services/sdk/windows/bin/{Flavor}/Windows 8/Managed/Microsoft.WindowsAzure.MobileServices.Managed.dll``` (where {Flavor} is Debug or Release).

### Sample Application: Doto

Doto is a simple, social todo list application that demonstrates the features of Windows Azure Mobile Services. You can find doto under ```/azure-mobile-services/samples/doto```.

### Need Help?

Be sure to check out the Mobile Services [Developer Forum](http://social.msdn.microsoft.com/Forums/en-US/azuremobile/) if you are having trouble. The Mobile Services product team actively monitors the forum and will be more than happy to assist you.

### Contribute Code or Provide Feedback

If you would like to become an active contributor to this project please follow the instructions provided in [Windows Azure Projects Contribution Guidelines](http://windowsazure.github.com/guidelines.html).

If you encounter any bugs with the library please file an issue in the [Issues](https://github.com/WindowsAzure/azure-mobile-services/issues) section of the project.

## Learn More
[Windows Azure Mobile Services Developer Center](http://www.windowsazure.com/en-us/develop/mobile)
