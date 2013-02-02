// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using Microsoft.Azure.Zumo.Win8.CSharp.Test;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    public class MobileServiceTableUrlBuilderTests : TestBase
    {
        [Test]
        public void GetUriFragmentWithTableNameTest()
        {
            Assert.AreEqual("tables/someTable", MobileServiceTableUrlBuilder.GetUriFragment("someTable"));
        }
 
        [Test]
        public void GetUriFragmentWithTableNameAndIdTest()
        {
            Assert.AreEqual("tables/someTable/5", MobileServiceTableUrlBuilder.GetUriFragment("someTable", 5));
            Assert.AreEqual("tables/someTable/12.2", MobileServiceTableUrlBuilder.GetUriFragment("someTable", 12.2));
            Assert.AreEqual("tables/someTable/hi", MobileServiceTableUrlBuilder.GetUriFragment("someTable", "hi"));
        }
 
        [Test]
        public void GetQueryStringTest()
        {
            var parameters = new Dictionary<string, string>() { { "x", "$y" }, { "&hello", "?good bye" }, { "a$", "b" } };
            Assert.AreEqual("x=%24y&%26hello=%3Fgood%20bye&a%24=b", MobileServiceTableUrlBuilder.GetQueryString(parameters));
            Assert.AreEqual(null, MobileServiceTableUrlBuilder.GetQueryString(null));
            //Assert.AreEqual(null, MobileServiceTableUrlBuilder.GetQueryString(new Dictionary<string, string>()));
        }
 
        [Test]
        public void GetQueryStringThrowsTest()
        {
            var parameters = new Dictionary<string, string>() { { "$x", "someValue" } };
            Assert.Throws<ArgumentException>(() => MobileServiceTableUrlBuilder.GetQueryString(parameters));
        }
 
        [Test]
        public void CombinePathAndQueryTest()
        {
            Assert.AreEqual("somePath?x=y&a=b", MobileServiceTableUrlBuilder.CombinePathAndQuery("somePath", "x=y&a=b"));
            Assert.AreEqual("somePath?x=y&a=b", MobileServiceTableUrlBuilder.CombinePathAndQuery("somePath", "?x=y&a=b"));
            Assert.AreEqual("somePath", MobileServiceTableUrlBuilder.CombinePathAndQuery("somePath", null));
            Assert.AreEqual("somePath", MobileServiceTableUrlBuilder.CombinePathAndQuery("somePath", ""));
        }
    }
}