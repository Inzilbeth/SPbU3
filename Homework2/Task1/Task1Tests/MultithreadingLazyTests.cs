using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using Task1Library;

namespace Task1Tests
{
    [TestFixture]
    public class MultithreadingLazyTests
    {
        [Test]
        public void RaceTestMT()
        {
            var totalTests = 10000;
            var totalThreads = 10;

            for (int j = 0; j < totalTests; j++)
            {
                var testObject = LazyFactory.CreateMultiThreaded(() => new List<int> { 1, 2, 3 });

                var threads = new Thread[totalThreads];
                var objects = new List<int>[totalThreads];

                for (var i = 0; i < totalThreads; i++)
                {
                    var localI = i;
                    threads[localI] = new Thread(() => objects[localI] = testObject.Get());
                }

                foreach (var thread in threads)
                {
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                for (int i = 1; i < totalThreads; i++)
                {
                    Assert.AreSame(objects[0], objects[i]);
                }
            }
        }
    }
}
