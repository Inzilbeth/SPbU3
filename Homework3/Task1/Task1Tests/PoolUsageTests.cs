using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using Task1;

namespace Task1Tests
{
    public class PoolUsageTests
    {
        const int POOL_SIZE = 1;
        private MyThreadPool pool;

        [SetUp]
        public void Setup()
        {
            pool = new MyThreadPool(POOL_SIZE);
        }

        [Test]
        public void EnqueueFunctionalityTest()
        {
            int size = 10;
            var tasks = new IMyTask<int>[10]; 
            
            for (int i = 0; i < size; i++)
            {
                var localI = i;
                tasks[localI] = pool.Enqueue(() => 1000 * (localI + 1));
            }

            for (int i = 0; i < size; i++)
            {
                Assert.AreEqual((i + 1) * 1000, tasks[i].Result);
            }
        }

        [Test]
        public void ContinueWithFunctionalityTest()
        {
            int size = 10;
            var tasks = new IMyTask<string>[10];

            for (int i = 0; i < size; i++)
            {
                var localI = i;
                tasks[localI] = pool.Enqueue(() => (localI + 1) * 10)
                    .ContinueWith(x => x.ToString());
            }

            for (int i = 0; i < size; i++)
            {
                Assert.AreEqual(((i + 1) * 10).ToString(), tasks[i].Result);
            }
        }

        [Test]
        public void SeveralContinuationsTest()
        {
            var task = pool.Enqueue(() => 10 + 2)
                .ContinueWith(x => x.ToString())
                .ContinueWith(x => x + "34")
                .ContinueWith(x => x.ToCharArray());

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(char.Parse((i + 1).ToString()), task.Result[i]);
            }
        }

        [Test]
        public void PoolSizeTest()
        {
            var stopWatch = new Stopwatch();
            var tasks = new IMyTask<int>[POOL_SIZE];

            stopWatch.Start();
            for (int i = 0; i < POOL_SIZE; i++)
            {
                var localI = i;
                tasks[localI] = pool.Enqueue(() =>
                {
                    Thread.Sleep(2000);
                    return localI;
                });
            }

            for (int i = 0; i < POOL_SIZE; i++)
            {
                if (i != tasks[i].Result)
                {
                    Assert.Fail("Values didn't match");
                }
            }
            stopWatch.Stop();

            // This is possible only if all 5 threads worked simultaneously
            Assert.IsTrue(stopWatch.ElapsedMilliseconds < 2000 * POOL_SIZE + 1000); 
        }

        [Test]
        public void ShutdownTestOnContinuedTasks()
        {
            var tasks = new IMyTask<int>[POOL_SIZE + 1];

            for (int i = 0; i < POOL_SIZE; i++)
            {
                var localI = i;
                tasks[localI] = pool.Enqueue(() =>
                {
                    Thread.Sleep(2000);
                    return localI * 10;
                }).ContinueWith(x => x * 10);
            }

            tasks[POOL_SIZE] = pool.Enqueue(() => { Thread.Sleep(2000); return 777; });

            pool.Shutdown();

            Thread.Sleep(7000);
            int temp;
            Assert.Throws<InvalidOperationException>(() => temp = tasks[POOL_SIZE].Result);
        }
        
        [Test]
        public void ShutdownTestOnDefaultTasks()
        {
            var task = pool.Enqueue(() =>
            {
                Thread.Sleep(1000);
                return 10 * 10;
            });

            var newTask = pool.Enqueue(() => { Thread.Sleep(2000); return 777; });

            pool.Shutdown();

            int t;
            Assert.Throws<InvalidOperationException>(() => t = newTask.Result);
        }
    }
}