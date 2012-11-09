using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Microsoft.Azure.Zumo.Win8.CSharp.Test
{
    public class TestBase
    {
        public const int Timeout = 60000;

        public void ThrowsAsync<T> (Func<Task> task) where T : Exception
        {
            try
            {
                Task t = task();
                t.WaitOrFail (Timeout);
                Assert.Fail (String.Format ("{0} not thrown", typeof(T).Name));
            }
            catch (AggregateException aex)
            {
                aex.AssertCaught<T>();
            }
            catch (T ex)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail (String.Format ("Unexpected exception {0} thrown instead of {1}.",
                                            ex.GetType().Name, typeof(T).Name));
            }
        }
    }
}

