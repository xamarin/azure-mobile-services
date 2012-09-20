using System;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    public static class TaskExtensions
    {
        public static void WaitOrFail (this Task self, int timeout)
        {
            if (!self.Wait (timeout))
                Assert.Fail();
        }

        public static T WaitOrFail<T> (this Task<T> self, int timeout)
        {
            if (!self.Wait (timeout))
            {
                Assert.Fail();
                return default(T);
            }

            return self.Result;
        }

        public static T AssertCaught<T> (this AggregateException self) where T : Exception
        {
            var aex = self.InnerException as AggregateException;
            if (aex != null)
                return aex.AssertCaught<T>();

            Assert.AreEqual (self.InnerExceptions.Count, 1, "A single exception was not thrown");
            Assert.That (self.InnerException, Is.InstanceOf<T>());
            return self.InnerException as T;
        }
    }
}