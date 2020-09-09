using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using Task1Library;

namespace Task1Tests
{
    public class LazyTests
    {
        [Test]
        public void IsNotInitializedBeforeCallST()
        {
            var testObject = LazyFactory.CreateSingleThreaded(() => 1);

            Assert.IsFalse(testObject.IsInitialized);
        }

        [Test]
        public void IsInitializedAfterCallST()
        {
            var testObject = LazyFactory.CreateSingleThreaded(() => 1);

            testObject.Get();

            Assert.IsTrue(testObject.IsInitialized);
        }

        [Test]
        public void DoesNotComputeMoreThanOnceST()
        {
            var count = 0;
            var testObject = LazyFactory.CreateSingleThreaded(() => { ++count; return 30; });

            testObject.Get();
            testObject.Get();
            testObject.Get();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void ReturnsCorrectValueST()
        {
            var testObject = LazyFactory.CreateSingleThreaded(() => 1);
            Assert.AreEqual(1, testObject.Get());
        }

        [Test]
        public void IsNotInitializedBeforeCallMT()
        {
            var testObject = LazyFactory.CreateMultiThreaded(() => 1);

            Assert.IsFalse(testObject.IsInitialized);
        }

        [Test]
        public void IsInitializedAfterCallMT()
        {
            var testObject = LazyFactory.CreateMultiThreaded(() => 1);

            testObject.Get();

            Assert.IsTrue(testObject.IsInitialized);
        }

        [Test]
        public void DoesNotComputeMoreThanOnceMT()
        {
            var count = 0;
            var testObject = LazyFactory.CreateMultiThreaded(() => { ++count; return 30; });

            testObject.Get();
            testObject.Get();
            testObject.Get();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void ReturnsCorrectValueMT()
        {
            var testObject = LazyFactory.CreateMultiThreaded<int>(() => 1);
            Assert.AreEqual(1, testObject.Get());
        }

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
