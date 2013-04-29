using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Data.Json;
using Xamarin.Auth;
using MonoTouch.UIKit;
using System.Drawing;

namespace Microsoft.WindowsAzure.MobileServices
{
    public sealed partial class MobileServiceClient
    {
        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="viewController" type="MonoTouch.UIKit.UIViewController">
        /// UIViewController used to display modal login UI on iPhone/iPods.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (UIViewController viewController, MobileServiceAuthenticationProvider provider)
        {
            return this.SendLoginAsync(default(RectangleF), viewController, provider, null);
        }

        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="viewController" type="MonoTouch.UIKit.UIViewController">
        /// UIViewController used to display modal login UI on iPhone/iPods.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (UIViewController viewController, MobileServiceAuthenticationProvider provider, JsonObject token)
        {
            return this.SendLoginAsync(default(RectangleF), viewController, provider, token);
        }

        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="rectangle" type="System.Drawing.RectangleF">
        /// The area in <paramref name="view"/> to anchor to.
        /// </param>
        /// <param name="view" type="MonoTouch.UIKit.UIView">
        /// UIView used to display a popover from on iPad.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (RectangleF rectangle, UIView view, MobileServiceAuthenticationProvider provider)
        {
            return this.SendLoginAsync(rectangle, view, provider, null);
        }

        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="rectangle" type="System.Drawing.RectangleF">
        /// The area in <paramref name="view"/> to anchor to.
        /// </param>
        /// <param name="view" type="MonoTouch.UIKit.UIView">
        /// UIView used to display a popover from on iPad.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (RectangleF rectangle, UIView view, MobileServiceAuthenticationProvider provider, JsonObject token)
        {
            return this.SendLoginAsync(rectangle, view, provider, token);
        }

        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="barButtonItem" type="MonoTouch.UIKit.UIBarButtonItem">
        /// UIBarButtonItem used to display a popover from on iPad.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (UIBarButtonItem barButtonItem, MobileServiceAuthenticationProvider provider)
        {
            return this.SendLoginAsync(default(RectangleF), barButtonItem, provider, null);
        }

        /// <summary>
        /// Log a user into a Mobile Services application given a provider name.
        /// </summary>
        /// <param name="barButtonItem" type="MonoTouch.UIKit.UIBarButtonItem">
        /// UIBarButtonItem used to display a popover from on iPad.
        /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (UIBarButtonItem barButtonItem, MobileServiceAuthenticationProvider provider, JsonObject token)
        {
            return this.SendLoginAsync(default(RectangleF), barButtonItem, provider, token);
        }

        internal Task<MobileServiceUser> SendLoginAsync(RectangleF rect, object view, MobileServiceAuthenticationProvider provider, JsonObject token = null)
        {
            if (this.LoginInProgress)
            {
                throw new InvalidOperationException(Resources.MobileServiceClient_Login_In_Progress);
            }
            
            if (!Enum.IsDefined(typeof(MobileServiceAuthenticationProvider), provider)) 
            {
                throw new ArgumentOutOfRangeException("provider");
            }

            string providerName = provider.ToString().ToLower();

            this.LoginInProgress = true;

            TaskCompletionSource<MobileServiceUser> tcs = new TaskCompletionSource<MobileServiceUser> ();

            if (token != null)
            {
                // Invoke the POST endpoint to exchange provider-specific token for a Windows Azure Mobile Services token

                this.RequestAsync("POST", LoginAsyncUriFragment + "/" + providerName, token)
                    .ContinueWith (t =>
                    {
                        this.LoginInProgress = false;

                        if (t.IsCanceled)
                            tcs.SetCanceled();
                        else if (t.IsFaulted)
                            tcs.SetException (t.Exception.InnerExceptions);
                        else
                        {
                            SetupCurrentUser (t.Result);
                            tcs.SetResult (this.CurrentUser);
                        }
                    });
            }
            else
            {
                // Launch server side OAuth flow using the GET endpoint

                Uri startUri = new Uri(this.ApplicationUri, LoginAsyncUriFragment + "/" + providerName);
                Uri endUri = new Uri(this.ApplicationUri, LoginAsyncDoneUriFragment);

                WebRedirectAuthenticator auth = new WebRedirectAuthenticator (startUri, endUri);
                UIViewController c = auth.GetUI();

                UIViewController controller = null;
                UIPopoverController popover = null;

                auth.Error += (o, e) =>
                {
                    this.LoginInProgress = false;

                    if (controller != null)
                        controller.DismissModalViewControllerAnimated (true);
                    if (popover != null)
                        popover.Dismiss (true);

                    Exception ex = e.Exception ?? new Exception (e.Message);
                    tcs.TrySetException (ex);
                };
                
                auth.Completed += (o, e) =>
                {
                    this.LoginInProgress = false;

                    if (controller != null)
                        controller.DismissModalViewControllerAnimated (true);
                    if (popover != null)
                        popover.Dismiss (true);

                    if (!e.IsAuthenticated)
                        tcs.TrySetCanceled();
                    else
                    {
                        SetupCurrentUser (JsonValue.Parse (e.Account.Properties["token"]));
                        tcs.TrySetResult (this.CurrentUser);
                    }
                };

                controller = view as UIViewController;
                if (controller != null)
                {
                    controller.PresentModalViewController (c, true);
                }
                else
                {
                    UIView v = view as UIView;
                    UIBarButtonItem barButton = view as UIBarButtonItem;

                    popover = new UIPopoverController (c);

                    if (barButton != null)
                        popover.PresentFromBarButtonItem (barButton, UIPopoverArrowDirection.Any, true);
                    else
                        popover.PresentFromRect (rect, v, UIPopoverArrowDirection.Any, true);
                }
            }
            
            return tcs.Task;
        }

        private void SetupCurrentUser (IJsonValue value)
        {
            IJsonValue response = value;

            this.CurrentUser = new MobileServiceUser (response.Get ("user").Get ("userId").AsString());
            // Get the Mobile Services auth token and user data
            this.CurrentUser.MobileServiceAuthenticationToken = response.Get (LoginAsyncAuthenticationTokenKey).AsString();
        }
    }
}