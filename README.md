# Windows Azure Mobile Services

With Windows Azure Mobile Services you can add a scalable backend to your connected client applications in minutes. To learn more, visit our [Developer Center](http://www.windowsazure.com/en-us/develop/mobile).

## Getting Started

If you are new to Mobile Services, you can get started by following our tutorials for connecting your Mobile Services cloud backend to [Windows Store apps](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started/) and [iOS apps](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started-ios/).

## Download Source Code

To get the source code of our SDKs and samples via **git** just type:

    git clone https://github.com/xamarin/azure-mobile-services.git
    cd azure-mobile-services
    git submodule init
    git submodule update

## Xamarin and Azure: Xamarin.iOS and Xamarin.Android Client SDK

Our client SDK makes it incredibly easy to use Mobile Services from your Xamarin.iOS and Xamarin.Android applications. You can download the source code using the instructions above and then you will find the SDKs under ```/azure-mobile-services/sdk/xamarin/ios``` and ```/azure-mobile-services/sdk/xamarin/android```.

### Prerequisities

The SDK requires Xamarin.iOS or Xamarin.Android.

### Running the Tests

The Xamarin.iOS SDK has a suite of unit tests. 

1. Open the ```/azure-mobile-services/sdk/ios/Microsoft.Azure.Zumo.iOS.sln``` solution file.
2. Right click on the ```ZumoTests``` project in the solution explorer and select ```Set as StartUp Project```.
3. Start the application
4. A Xamarin.iOS application will appear, press ```Run Everything```

### Sample Application: Todo

Todo is a simple todo list application that demonstrates some features of Windows Azure Mobile Services. You can find todo under ```/azure-mobile-services/samples/todo```.

### Need Help?

Be sure to check out the Mobile Services [Developer Forum](http://social.msdn.microsoft.com/Forums/en-US/azuremobile/) if you are having trouble. The Mobile Services product team actively monitors the forum and will be more than happy to assist you.

### Contribute Code or Provide Feedback

If you would like to become an active contributor to this project please follow the instructions provided in [Windows Azure Projects Contribution Guidelines](http://windowsazure.github.com/guidelines.html).

If you encounter any bugs with the library please file an issue in the [Issues](https://github.com/xamarin/azure-mobile-services/issues) section of the project.

## Learn More
[Windows Azure Mobile Services Developer Center](http://www.windowsazure.com/en-us/develop/mobile)
