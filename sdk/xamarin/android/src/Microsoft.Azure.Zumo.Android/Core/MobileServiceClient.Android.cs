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

            TaskCompletionSource<MobileServiceUser> tcs = new TaskCompletionSource<MobileServiceUser> ();

            if (token != null)
            {
                // Invoke the POST endpoint to exchange provider-specific token for a Windows Azure Mobile Services token

                this.RequestAsync("POST", LoginAsyncUriFragment + "/" + providerName, token)
                    .ContinueWith (t =>
                    {
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
                auth.Error += (o, e) =>
                {
                    Exception ex = e.Exception ?? new Exception (e.Message);
                    tcs.TrySetException (ex);
                };
                
                auth.Completed += (o, e) =>
                {
                    if (!e.IsAuthenticated)
                        tcs.TrySetCanceled();
                    else
                    {
                        SetupCurrentUser (JsonValue.Parse (e.Account.Properties["token"]));
                        tcs.TrySetResult (this.CurrentUser);
                    }
                };

                Intent intent = auth.GetUI (context);
                context.StartActivity (intent);
            }

            return tcs.Task;
        }

        private void SetupCurrentUser (IJsonValue value)
        {
            IJsonValue response = value;
            // Get the Mobile Services auth token and user data
            CurrentUser = new MobileServiceUser (response.Get ("user").Get ("userId").AsString());
            CurrentUser.MobileServiceAuthenticationToken = response.Get (LoginAsyncAuthenticationTokenKey).AsString();
        }
    }
}