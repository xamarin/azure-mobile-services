// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using Microsoft.WindowsAzure.MobileServices;
using NUnit.Framework;

namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    public class ZumoUserTests
    {
        [Test]
        public void CreateUser()
        {
            string id = "qwrdsjjjd8";
            MobileServiceUser user = new MobileServiceUser(id);
            Assert.AreEqual(id, user.UserId);

            new MobileServiceUser(null);
            new MobileServiceUser("");
        }
    }
}
