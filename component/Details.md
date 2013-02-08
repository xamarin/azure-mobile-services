Mobile Services is a quick and easy way to add a scalable and secure backend hosted in the Windows Azure cloud to your iOS apps. 

## Getting Started

To use Mobile Services with your iOS app, you will need a Windows Azure account.  If you already have an account, login to the [Windows Azure management portal](https://manage.windowsazure.com/).  If you are new to Windows Azure, you can sign up for a 90-day free trial [here](https://www.windowsazure.com/en-us/pricing/free-trial/).

To create a new Mobile Service after you've logged into the [management portal](https://manage.windowsazure.com/), select 'New' --> 'Compute' --> 'Mobile Service' --> 'Create.'  

![](WAMS-New.png)

Even though you will write the majority of your application in your preferred IDE, the management portal provides an easy way to work with three key Mobile Services features: storing data in the cloud, settuing up user authentication via third party services like Facebook, and sending push notifications.

You can find the full Getting Started with Mobile Services tutorial [here](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started/).

## Data
Store data in the cloud with Windows Auzre SQL database, Blob Storage, and Table Storage.

Set up a new SQL database or connect to an existing one.

![](WAMS-SQLdb1.png)

![](WAMS-SQLdb2.png)

To access blob storage or table storage, copy the appropirate code snippet below into a script.  By doing so you will  obtain a reference to a Windows Azure blob or table (after which you can query it or insert data into it).

Blob:

```js
var azure = require('azure');
var blobService = azure.createBlobService("<< account name >>", "<< access key >>");
```

Table:

```js
var azure = require('azure');
var tableService = azure.createTableService("<< account name >>", "<< access key >>");
```

You can find the full Getting Started with Data [here](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started-with-data-dotnet/).

##Auth
You can authenticate users through thier Facebook, Twitter, Microsoft, or Google credentials. (A single app can simultaneously support multiple forms of identity so you can of course offer your users a choice of how to login.) 

Copy the Client ID and Client  Secret to the appropriate place in the Identity tab. 

![](WAMS-userauth.png)

To allow your users to login with their Facebook credentials, for example, you'd use this code: 

```csharp
App.MobileService
	.LoginAsync (MobileServiceAuthenticationProvider.Facebook)
	.ContinueWith (t => {
		var user = t.Result;
		...
	});
```

You can find the full Getting Started with Authentication tutorial [here](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started-with-users-dotnet/).

##Push
To send push notifications, upload your developer certificate under the authentication tab in the Windows Azure portal.

![](WAMS-push1.png)

Mobile Services allows you to easily send push notifications via Apple Push Notification Services (APNS)

```js
push.apns.send(devicetoken, { alert: "Hello to Apple World from Mobile Services!"});
```

You can also provide a script (invoked periodically while your service is active) to check for expired device totkens and channels.

![](WAMS-push2.png)

```js
push.apns.getFeedback ({ 
	success: function (results) {
				// results is an array of objects with a deviceToken and time properties 
	}
});
```

You can find the full Getting Started with Push Notifications tutorial [here](https://www.windowsazure.com/en-us/develop/mobile/tutorials/get-started-with-push-ios/).


To learn about more Mobile Services, visit the [Windows Azure Mobile Developer Center](https://www.windowsazure.com/en-us/develop/mobile/).
