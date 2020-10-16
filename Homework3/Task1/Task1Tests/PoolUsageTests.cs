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
            var handler = new ManualResetEvent(true);
            var count = 0;

            stopWatch.Start();
            for (int i = 0; i < POOL_SIZE; i++)
            {
                var localI = i;
                tasks[localI] = pool.Enqueue(() =>
                {
                    count++;
                    handler.WaitOne();
                    handler.Reset();
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

            handler.Close();
            Assert.AreEqual(POOL_SIZE, count);
        }

        [Test]
        public void ShutdownTestOnContinuedTask()
        {
            var preTask = pool.Enqueue(() => 10);

            pool.Shutdown();
            //Thread.Sleep(1000);
            //IMyTask<int> temp;
            //Assert.Throws<InvalidOperationException>(() => temp = preTask.ContinueWith(x => x + 10));
        }
        
        [Test]
        public void ShutdownTestOnDefaultTasks()
        {
            var preTask = pool.Enqueue(() => 777);

            pool.Shutdown();

            IMyTask<int> temp; 
         
            Assert.Throws<InvalidOperationException>(() => temp = pool.Enqueue(() => 111));
        }
    }
}