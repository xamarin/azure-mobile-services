using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.Content;
using Windows.Data.Json;
using Xamarin.Auth;

namespace Microsoft.WindowsAzure.MobileServices
{
	public sealed partial class MobileServiceClient
	{
	    /// <summary>
	    /// Log a user into a Mobile Services application given a provider name.
	    /// </summary>
	    /// <param name="context" type="Android.Content.Context">
	    /// Context used to launch login UI.
	    /// </param>
	    /// <param name="provider" type="MobileServiceAuthenticationProvider">
	    /// Authentication provider to use.
	    /// </param>
	    /// <returns>
	    /// Task that will complete when the user has finished authentication.
	    /// </returns>
	    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync (Context context, MobileServiceAuthenticationProvider provider)
        {
            return this.SendLoginAsync(context, provider, null);
        }

		 /// <summary>
        /// Log a user into a Mobile Services application given a provider name and optional token object.
        /// </summary>
	    /// <param name="context" type="Android.Content.Context">
	    /// Context used to launch login UI.
	    /// </param>
		/// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <param name="token" type="JsonObject">
        /// Optional, provider specific object with existing OAuth token to log in with.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is more appropriate than LogOn for our usage.")]
        public Task<MobileServiceUser> LoginAsync(Context context, MobileServiceAuthenticationProvider provider, JsonObject token)
        {
            return this.SendLoginAsync(context, provider, token);
        }

		/// <summary>
        /// Log a user into a Mobile Services application given a provider name and optional token object.
        /// </summary>
	    /// <param name="context" type="Android.Content.Context">
	    /// Context used to launch login UI.
	    /// </param>
        /// <param name="provider" type="MobileServiceAuthenticationProvider">
        /// Authentication provider to use.
        /// </param>
        /// <param name="token" type="JsonObject">
        /// Optional, provider specific object with existing OAuth token to log in with.
        /// </param>
        /// <returns>
        /// Task that will complete when the user has finished authentication.
        /// </returns>
        internal Task<MobileServiceUser> SendLoginAsync(Context context, MobileServiceAuthenticationProvider provider, JsonObject token = null)
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

		    Task<MobileServiceUser> task = null;

            if (token != null)
            {
                // Invoke the POST endpoint to exchange provider-specific token for a Windows Azure Mobile Services token

                task = this.RequestAsync("POST", LoginAsyncUriFragment + "/" + providerName, token)
                    .ContinueWith<MobileServiceUser> (ContinueRequest);
            }
            else
            {
                // Launch server side OAuth flow using the GET endpoint

                Uri startUri = new Uri(this.ApplicationUri, LoginAsyncUriFragment + "/" + providerName);
                Uri endUri = new Uri(this.ApplicationUri, LoginAsyncDoneUriFragment);

                TaskCompletionSource<IJsonValue> tcs = new TaskCompletionSource<IJsonValue>();

                WebRedirectAuthenticator auth = new WebRedirectAuthenticator (startUri, endUri);
                auth.Error += (o, e) =>
                {
                    Exception ex = e.Exception ?? new Exception (e.Message);
                    tcs.TrySetException (ex);
                };

                auth.Completed += (o, e) =>
                {
                    tcs.SetResult (JsonValue.Parse (e.Account.Properties["token"]));
                };

                task = tcs.Task.ContinueWith<MobileServiceUser> (ContinueRequest);

                Intent intent = auth.GetUI (context);
                context.StartActivity (intent);
            }

		    return task;
        }

        private MobileServiceUser ContinueRequest (Task<IJsonValue> task)
        {
            this.LoginInProgress = false;

            if (!task.IsFaulted)
            {
                IJsonValue response = task.Result;
                // Get the Mobile Services auth token and user data
                this.currentUserAuthenticationToken = response.Get (LoginAsyncAuthenticationTokenKey).AsString();
                this.CurrentUser = new MobileServiceUser (response.Get ("user").Get ("userId").AsString());
            }

            return this.CurrentUser;
        }
	}
}