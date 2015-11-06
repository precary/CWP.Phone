using System;
using Xunit;

namespace CWP.Phone.Test
{
    static class AssertHelper
    {
        public static void Throws(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                return;
            }

            Assert.False(true, "No exception was thrown.");
        }
    }
}