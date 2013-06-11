// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.WindowsAzure.MobileServices
{
    /// <summary>
    /// Represents an HTTP request that can be manipulated by IServiceFilters
    /// and is backed by an HttpWebRequest.
    /// </summary>
    internal sealed class ServiceFilterRequest : IServiceFilterRequest
    {
        /// <summary>
        /// Initializes a new instance of the ServiceFilterRequest class.
        /// </summary>
        public ServiceFilterRequest()
        {
            this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Gets or sets the HTTP method for the request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URI for the request.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets a collection of headers for the request.
        /// </summary>
        /// <remarks>
        /// Because we're using HttpWebRequest behind the scenes, there are a
        /// few headers like Accept and ContentType which must be set using the
        /// properties defined on the interface rather than via Headers.
        /// </remarks>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the type of responses accepted.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Gets or sets the body of the request.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the type of the body's content for the request.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Create an HttpWebRequest that represents the request.
        /// </summary>
        /// <returns>An HttpWebRequest.</returns>
        private Task<HttpWebRequest> CreateHttpWebRequestAsync()
        {
            // Create the request
            HttpWebRequest request = new HttpWebRequest (this.Uri);
            request.Method = this.Method;
            request.Accept = this.Accept;
            
            // Copy over any headers
            foreach (KeyValuePair<string, string> header in this.Headers)
            {
                // This could possibly throw if the user sets one of the
                // known headers on HttpWebRequest.  There's no way we can
                // recover so we'll just let the exception bubble up to the
                // user.  There will be no way they can work around this
                // problem unless we add that particular header to the
                // IServiceFilterRequest interface.
                request.Headers[header.Key] = header.Value;
            }

			TaskCompletionSource<HttpWebRequest> tcs = new TaskCompletionSource<HttpWebRequest>();

            // Copy the request body
            if (!string.IsNullOrEmpty(this.Content))
            {
                request.ContentType = this.ContentType;

	            Task<Stream>.Factory.FromAsync (request.BeginGetRequestStream, request.EndGetRequestStream, null)
		            .ContinueWith (t =>
		            {
					    if (t.IsFaulted)
					    {
						    tcs.SetException (t.Exception.InnerExceptions);
						    return;
					    }

			            using (StreamWriter writer = new StreamWriter (t.Result))
				            writer.Write (Content);

						t.Result.Dispose();
			            tcs.SetResult (request);
		            });
            }
			else
	            tcs.SetResult (request);
            
            return tcs.Task;
        }

        /// <summary>
        /// Get the HTTP response for this request.
        /// </summary>
        /// <returns>The HTTP response.</returns>
        public Task<IServiceFilterResponse> GetResponse()
        {
            return this.GetResponseAsync();
        }

        /// <summary>
        /// Get the HTTP response for this request.
        /// </summary>
        /// <returns>The HTTP response.</returns>
        private Task<IServiceFilterResponse> GetResponseAsync()
        {
	        return CreateHttpWebRequestAsync().ContinueWith (treq =>
	        {
		        if (treq.IsFaulted)
		        {
			        WebException ex = treq.Exception.InnerExceptions.OfType<WebException>().FirstOrDefault();
			        if (ex == null)
				        throw treq.Exception;

			        return new ServiceFilterResponse (ex.Response as HttpWebResponse, (ServiceFilterResponseStatus)ex.Status);
		        }

		        return (IServiceFilterResponse)Task<WebResponse>.Factory.FromAsync (treq.Result.BeginGetResponse, treq.Result.EndGetResponse, null)
			        .ContinueWith (tres =>
			        {
				        if (tres.IsFaulted)
				        {
					        WebException ex = tres.Exception.InnerExceptions.OfType<WebException>().FirstOrDefault();
					        if (ex == null)
						        throw tres.Exception;

					        return new ServiceFilterResponse (ex.Response as HttpWebResponse, (ServiceFilterResponseStatus)ex.Status);
				        }

				        return new ServiceFilterResponse (tres.Result as HttpWebResponse, ServiceFilterResponseStatus.Success);
			        }).Result;
	        });
        }
    }
}
