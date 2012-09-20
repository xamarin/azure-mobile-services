// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    /// <summary>
    /// ServiceFilter that allows a test to control the HTTP pipeline and
    /// analyze a request and provide a set response.
    /// </summary>
    public class TestServiceFilter : IServiceFilter
    {
        public TestServiceFilter()
        {
            Response = new TestServiceResponse();
        }

        public IServiceFilterRequest Request { get; private set; }
        public TestServiceResponse Response { get; private set; }

        public Task<IServiceFilterResponse> Handle(IServiceFilterRequest request, IServiceFilterContinuation continuation)
        {
            Request = request;
            var tcs = new TaskCompletionSource<IServiceFilterResponse>();
            tcs.SetResult (Response);
            return tcs.Task;
        }
    }

    public class TestServiceResponse : IServiceFilterResponse
    {
        public TestServiceResponse()
        {
            Headers = new Dictionary<string, string>();
            StatusCode = 200;
            StatusDescription = "";
            ContentType = "application/json";
        }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string ContentType { get; set; }

        public IDictionary<string, string> Headers { get; private set; }

        public string Content { get; set; }

        public ServiceFilterResponseStatus ResponseStatus { get; set; }
    }
}
