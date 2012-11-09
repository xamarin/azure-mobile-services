// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.WindowsAzure.MobileServices;
using Windows.Data.Json;
using MonoTouch.Foundation;


namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    public class ZumoServiceTests : TestBase
    {
        /// <summary>
        /// Verify we have an installation ID created whenever we use a ZUMO
        /// service. 
        /// </summary>
        [Test]
        public void InstallationId()
        {
            MobileServiceClient service = new MobileServiceClient("http://test.com");

            var defaults = NSUserDefaults.StandardUserDefaults;
            string settings = defaults.StringForKey ("MobileServices.Installation.config");
            string id = JsonValue.Parse(settings).Get("applicationInstallationId").AsString();
            Assert.IsNotNull(id);
        }

        [Test]
        public void Construction()
        {
            string appUrl = "http://www.test.com/";
            string appKey = "secret...";

            MobileServiceClient service = new MobileServiceClient(new Uri(appUrl), appKey);
            Assert.AreEqual(appUrl, service.ApplicationUri.ToString());
            Assert.AreEqual(appKey, service.ApplicationKey);

            service = new MobileServiceClient(appUrl, appKey);
            Assert.AreEqual(appUrl, service.ApplicationUri.ToString());
            Assert.AreEqual(appKey, service.ApplicationKey);

            service = new MobileServiceClient(new Uri(appUrl));
            Assert.AreEqual(appUrl, service.ApplicationUri.ToString());
            Assert.AreEqual(null, service.ApplicationKey);

            service = new MobileServiceClient(appUrl);
            Assert.AreEqual(appUrl, service.ApplicationUri.ToString());
            Assert.AreEqual(null, service.ApplicationKey);

            Uri none = null;
            Assert.Throws<ArgumentNullException>(() => new MobileServiceClient(none));
            Assert.Throws<UriFormatException>(() => new MobileServiceClient("not a valid uri!!!@#!@#"));
        }

        [Test]
        public void WithFilter()
        {
            string appUrl = "http://www.test.com/";
            string appKey = "secret...";
            TestServiceFilter hijack = new TestServiceFilter();

            MobileServiceClient service =
                new MobileServiceClient(new Uri(appUrl), appKey)
                .WithFilter(hijack);

            // Ensure properties are copied over
            Assert.AreEqual(appUrl, service.ApplicationUri.ToString());
            Assert.AreEqual(appKey, service.ApplicationKey);

            // Set the filter to return an empty array
            hijack.Response.Content = new JsonArray().Stringify();

            service.GetTable("foo").ReadAsync("bar")
                .ContinueWith (t =>
                {
                    // Verify the filter was in the loop
                    Assert.That (hijack.Request.Uri.ToString(), Is.StringStarting (appUrl));
                    
                    Assert.Throws<ArgumentNullException>(() => service.WithFilter(null));
                }).WaitOrFail (Timeout);
            
        }

        [Test]
        public void LoginAsync()
        {
            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient("http://www.test.com", "secret...")
                .WithFilter(hijack);

            // Send back a successful login response
            hijack.Response.Content =
                new JsonObject()
                    .Set("authenticationToken", "rhubarb")
                    .Set("user",
                        new JsonObject()
                            .Set("userId", "123456")).Stringify();
            service.LoginAsync("donkey").ContinueWith (t =>
            {
                var current = t.Result;
                Assert.IsNotNull(current);
                Assert.AreEqual("123456", current.UserId);
                Assert.That (hijack.Request.Uri.ToString(), Is.StringEnding ("login?mode=authenticationToken"));
                string input = JsonValue.Parse(hijack.Request.Content).Get("authenticationToken").AsString();
                Assert.AreEqual("donkey", input);
                Assert.AreEqual("POST", hijack.Request.Method);
                Assert.AreSame(current, service.CurrentUser);
                
                // Verify that the user token is sent with each request
                service.GetTable("foo").ReadAsync("bar").ContinueWith (rt =>
                {
                    var response = rt.Result;

                    Assert.AreEqual("rhubarb", hijack.Request.Headers["X-ZUMO-AUTH"]);
                    
                    // Verify error cases
                    ThrowsAsync<ArgumentNullException>(() => service.LoginAsync(null));
                    ThrowsAsync<ArgumentException>(() => service.LoginAsync(""));
                    
                    // Send back a failure and ensure it throws
                    hijack.Response.Content =
                        new JsonObject().Set("error", "login failed").Stringify();
                    hijack.Response.StatusCode = 401;
                    ThrowsAsync<InvalidOperationException>(() => service.LoginAsync("donkey"));
                }).WaitOrFail (Timeout);

            }).WaitOrFail (Timeout);
        }

        [Test]
        public void Logout()
        {
            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient("http://www.test.com", "secret...")
                .WithFilter(hijack);

            // Send back a successful login response
            hijack.Response.Content =
                new JsonObject()
                    .Set("authenticationToken", "rhubarb")
                    .Set("user",
                        new JsonObject()
                            .Set("userId", "123456")).Stringify();

            service.LoginAsync("donkey").ContinueWith (t =>
            {
                Assert.IsNotNull(service.CurrentUser);
                
                service.Logout();
                Assert.IsNull(service.CurrentUser);
            }).WaitOrFail (Timeout);
        }

        [Test]
        public void StandardRequestFormat()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";
            string query = "$filter=id eq 12";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);

            hijack.Response.Content =
                new JsonArray()
                    .Append(new JsonObject().Set("id", 12).Set("value", "test"))
                    .Stringify();

            service.GetTable(collection).ReadAsync(query).ContinueWith (t => {
                Assert.IsNotNull(hijack.Request.Headers["X-ZUMO-INSTALLATION-ID"]);
                Assert.AreEqual("secret...", hijack.Request.Headers["X-ZUMO-APPLICATION"]);
                Assert.AreEqual("application/json", hijack.Request.Accept);
            });
        }

        [Test]
        public void ErrorMessageConstruction()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";
            string query = "$filter=id eq 12";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);

            // Verify the error message is correctly pulled out
            hijack.Response.Content =
                new JsonObject()
                    .Set("error", "error message")
                    .Set("other", "donkey")
                    .Stringify();
            hijack.Response.StatusCode = 401;
            hijack.Response.StatusDescription = "YOU SHALL NOT PASS.";
            try
            {
                service.GetTable(collection).ReadAsync(query).WaitOrFail (Timeout);
            }
            catch (AggregateException aex)
            {
                var ex = aex.AssertCaught<InvalidOperationException>();
                Assert.That (ex.Message, Contains.Substring ("error message"));
            }

            // Verify all of the exception parameters
            hijack.Response.Content =
                new JsonObject()
                    .Set("error", "error message")
                    .Set("other", "donkey")
                    .Stringify();
            hijack.Response.StatusCode = 401;
            hijack.Response.StatusDescription = "YOU SHALL NOT PASS.";
            try
            {
                service.GetTable(collection).ReadAsync(query).WaitOrFail (Timeout);
            }
            catch (AggregateException aex)
            {
                var ex = aex.AssertCaught<MobileServiceInvalidOperationException>();
                Assert.That (ex.Message, Is.StringStarting ("error message"));
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.Response.StatusCode);
                Assert.That (ex.Response.Content, Contains.Substring ("donkey"));
                Assert.That (ex.Request.Uri.ToString(), Is.StringStarting (appUrl));
                Assert.AreEqual("YOU SHALL NOT PASS.", ex.Response.StatusDescription);
            }

            // If no error message in the response, we'll use the
            // StatusDescription instead
            hijack.Response.Content =
                new JsonObject()
                    .Set("other", "donkey")
                    .Stringify();
            try
            {
                service.GetTable(collection).ReadAsync(query).WaitOrFail (Timeout);
            }
            catch (AggregateException aex)
            {
                var ex = aex.AssertCaught<InvalidOperationException>();
                Assert.That (ex.Message, Is.StringStarting ("YOU SHALL NOT PASS."));
            }
        }

        public class Person
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void ReadAsync()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";
            string query = "$filter=id eq 12";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);

            hijack.Response.Content =
                new JsonArray()
                    .Append(new JsonObject().Set("id", 12).Set("value", "test"))
                    .Stringify();

            service.GetTable(collection).ReadAsync(query).ContinueWith (t => {
                Assert.That (hijack.Request.Uri.ToString(), Contains.Substring (collection));
                Assert.That (hijack.Request.Uri.ToString(), Is.StringEnding (query));
                
                ThrowsAsync<ArgumentNullException>(() => service.GetTable(null).ReadAsync(query));
                ThrowsAsync<ArgumentException>(() => service.GetTable("").ReadAsync(query));
            }).WaitOrFail (Timeout);
        }

        [Test]
        public void ReadAsyncGeneric()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);
            hijack.Response.Content =
                new JsonArray()
                    .Append(new JsonObject().Set("id", 12).Set("Name", "Bob"))
                    .Stringify();

            IMobileServiceTable<Person> table = service.GetTable<Person>();
            table.Where(p => p.Id == 12).ToListAsync().ContinueWith (t => {
                var people = t.Result;
                Assert.AreEqual(1, people.Count);
                Assert.AreEqual(12L, people[0].Id);
                Assert.AreEqual("Bob", people[0].Name);
            }).WaitOrFail (Timeout);
        }

        [Test]
        public void LookupAsync()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);
            hijack.Response.Content =
                new JsonObject()
                    .Set("id", 12)
                    .Set("Name", "Bob")
                    .Stringify();

            IMobileServiceTable<Person> table = service.GetTable<Person>();
            table.LookupAsync(12).ContinueWith (t => {
                var bob = t.Result;

                Assert.AreEqual(12L, bob.Id);
                Assert.AreEqual("Bob", bob.Name);
                
                hijack.Response.StatusCode = 404;
                bool thrown = false;
                try
                {
                    Task<Person> lookup = table.LookupAsync (12);
                    lookup.WaitOrFail (Timeout);
                    bob = lookup.Result;
                }
                catch (AggregateException aex)
                {
                    aex.AssertCaught<InvalidOperationException>();
                    thrown = true;
                }
                Assert.IsTrue(thrown, "Exception should be thrown on a 404!");
            }).WaitOrFail (Timeout);
        }

        [Test]
        public void InsertAsync()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);
                
            JsonObject obj = new JsonObject().Set("value", "new");
            hijack.Response.Content =
                new JsonObject().Set("id", 12).Set("value", "new").Stringify();
            service.GetTable(collection).InsertAsync(obj).WaitOrFail (Timeout);

            Assert.AreEqual(12, obj.Get("id").AsInteger());
            Assert.That (hijack.Request.Uri.ToString(), Contains.Substring (collection));

            ThrowsAsync<ArgumentNullException>(
                () => service.GetTable(collection).InsertAsync(null));
            
            // Verify we throw if ID is set on both JSON and strongly typed
            // instances
            ThrowsAsync<ArgumentException>(
                () => service.GetTable(collection).InsertAsync(
                    new JsonObject().Set("id", 15)));
            ThrowsAsync<ArgumentException>(
                () => service.GetTable<Person>().InsertAsync(
                    new Person() { Id = 15 }));
        }

        [Test]
        public void InsertAsyncThrowsIfIdExists()
        {
            string appUrl = "http://www.test.com";
            string collection = "tests";
            MobileServiceClient service = new MobileServiceClient(appUrl);

            // Verify we throw if ID is set on both JSON and strongly typed
            // instances
            ThrowsAsync<ArgumentException>(
                () => service.GetTable(collection).InsertAsync(
                    new JsonObject().Set("id", 15)));
            ThrowsAsync<ArgumentException>(
                () => service.GetTable<Person>().InsertAsync(
                    new Person() { Id = 15 }));
        }

        [Test]
        public void UpdateAsync()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);

            JsonObject obj = new JsonObject().Set("id", 12).Set("value", "new");
            hijack.Response.Content =
                new JsonObject()
                    .Set("id", 12)
                    .Set("value", "new")
                    .Set("other", "123")
                    .Stringify();
            IMobileServiceTable table = service.GetTable(collection);
            table.UpdateAsync(obj).WaitOrFail (Timeout);

            Assert.AreEqual("123", obj.Get("other").AsString());
            Assert.That (hijack.Request.Uri.ToString(), Contains.Substring (collection));

            ThrowsAsync<ArgumentNullException>(() => table.UpdateAsync(null));
            ThrowsAsync<ArgumentException>(() => table.UpdateAsync(new JsonObject()));
        }

        [Test]
        public void DeleteAsync()
        {
            string appUrl = "http://www.test.com";
            string appKey = "secret...";
            string collection = "tests";

            TestServiceFilter hijack = new TestServiceFilter();
            MobileServiceClient service = new MobileServiceClient(appUrl, appKey)
                .WithFilter(hijack);

            JsonObject obj = new JsonObject().Set("id", 12).Set("value", "new");
            IMobileServiceTable table = service.GetTable(collection);
            table.DeleteAsync(obj).WaitOrFail (Timeout);
                
            Assert.That (hijack.Request.Uri.ToString(), Contains.Substring (collection));

            ThrowsAsync<ArgumentNullException>(() => table.DeleteAsync(null));
            ThrowsAsync<ArgumentException>(() => table.DeleteAsync(new JsonObject()));
        }
    }
}
