## Sign up for Windows Azure
To build an iOS app using Windows Azure Mobile Services, you'll first need to either login to your Windows Azure account at https://manage.windowsazure.com or create a new account.  You can sign up for a 90-day free trial at https://www.windowsazure.com/en-us/pricing/free-trial/.  

## Create a New Mobile Service
Every Windows Azure subscription receives free Mobile Services for up to 10 apps. 

To create your first Mobile Service, login to your account at https://manage.windowsazure.com and click New on the bottom left portion of the page. You will then have the option to either create a new SQL database or connect to an existing one.

![](WAMS-Create.png)

## Connect a Mobile Service to your MonoTouch app written in C# 

After you've created a Mobile Service, use the following to connect your project:

```csharp
using Microsoft.WindowsAzure.MobileServices;

...

public static MobileServiceClient MobileService = new MobileServiceClient (
	"https://yourMobileServiceName.azure-mobile.net/", 
	"YOUR_APPLICATION_KEY"
);
```

To work with data in a table (here we're using a sample table called 'TodoItem', add the following class to your project:


To store data in that table, use the following code snippet (originally from the [September 2012 announcement](http://blog.xamarin.com/xamarin-partners-with-microsoft-to-support-azure-mobile-services-on-android-and-ios/) of the Xamarin and Windows Azure partnership):

```csharp 
public class TodoItem
{
		public int Id { get; set; }

		[DataMember (Name = "text")]
		public string Text { get; set; }

		[DataMember (Name = "complete")]
		public bool Complete { get; set; }
}
...

var table = MobileService.GetTable<TodoItem> ();
table.Where (ti => !ti.Complete)
     .ToListAsync ()
     .ContinueWith (t => items = t.Result, scheduler);
```

##Server-Side Scripts
Mobile Services allows you to add business logic to CRUD operations through secure server-side scripts.  Currently, scripts must be written in JavaScript.

To add a script, navigate to the 'DATA' tab on the dashbaord and select a table.

![](WAMS-Script1.png)

Then, under the 'SCRIPT' tab, choose either Insert, Update, Delete, or Read from the dropdown menu and copy in your script.  You can find samples for common scripts at http://msdn.microsoft.com/en-us/library/windowsazure/jj591477.aspx.

![](WAMS-Script2.png)

If you'd like to schedule a script to run periodically (rather than when triggerd by a particular event), visit the 'SCHEDULER' tab on the main dashboard and click 'Create a Scheduled Job.'  Then, set the interval at which you would like the script to run.

![](WAMS-Scheduler2.png)

Once you write the script, click 'Save' then 'Run Once.'  Check the 'LOGS' tab on the main dashboard for any errors.  If you're error-free, be sure to return to the 'SCHEDULER' tab and click 'Enable.'

![](WAMS-Scheduler3.png)

### Documentation

- Tutorials: https://www.windowsazure.com/en-us/develop/mobile/resources/
- Developer Center: http://www.windowsazure.com/mobile
- API Library: http://msdn.microsoft.com/en-us/library/windowsazure/jj710108.aspx
- Mobile Services GitHub Repo: https://github.com/WindowsAzure/azure-mobile-services
- Xamarin Mobile Services client framework GitHub Repo: https://github.com/xamarin/azure-mobile-services

### Contact

- Developer Forum: http://social.msdn.microsoft.com/Forums/en-US/azuremobile/threads
- Feature Requests: http://mobileservices.uservoice.com
- Contact: [mobileservices@microsoft.com](mailto:mobileservices@microsoft.com)
- Twitter: [@joshtwist](http://twitter.com/joshtwist) [@cloudnick](http://twitter.com/cloudnick) [@chrisrisner](http://twitter.com/chrisrisner) [@mlunes90](http://twitter.com/mlunes90)

### Legal 

- Terms & Conditions: http://www.windowsazure.com/en-us/support/legal/
