using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Task1
{
    /// <summary>
    /// Thread pool to perform tasks simultaneously.
    /// </summary>
    public class MyThreadPool
    {
        public delegate void AbortionHandler();
        public event AbortionHandler onShutdown;

        private Thread[] threads;
        private ConcurrentQueue<Action> queue;
        
        private ManualResetEvent reset;
        private CancellationTokenSource cancelTokenSource;

        /// <summary>
        /// Sets up a pool with specified amount of threads.
        /// </summary>
        /// <param name="size">Amount of threads initialized.</param>
        public MyThreadPool(int size)
        {
            threads = new Thread[size];
            queue = new ConcurrentQueue<Action>();

            cancelTokenSource = new CancellationTokenSource();
            reset = new ManualResetEvent(true);

            var token = cancelTokenSource.Token;

            for (int i = 0; i < size; i++)
            {
                var localI = i;
                threads[localI] = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (queue.TryDequeue(out var action))
                        {
                            action.Invoke();
                        }
                        else
                        {
                            reset.WaitOne();
                            reset.Reset();
                        }
                    }

                    return;
                });

                threads[localI].Name = $"WT #{localI}";
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start();
            }
        }

        /// <summary>
        /// Submits a new task to the thread pool using a function without arguments.
        /// </summary>
        /// <typeparam name="TResult">Type of task's result value.</typeparam>
        /// <param name="func">Function used to calculate the result.</param>
        /// <returns>A reference to the generated task.</returns>
        public IMyTask<TResult> Enqueue<TResult>(Func<TResult> func)
        {
            var myTask = new MyTask<TResult>(func, this);

            onShutdown += myTask.Abort;
            queue.Enqueue(myTask.Calculate);
            reset.Set();
            return myTask;
        }

        /// <summary>
        /// Submits a new task to the thread pool using a function with one argument,
        /// used to generate tasks based on another tasks.
        /// </summary>
        /// <typeparam name="TResult">Type of old task's result value.</typeparam>
        /// <typeparam name="TNewResult">Type of new task's result value.</typeparam>
        /// <param name="func">Function used to calculate the result.</param>
        /// <param name="oldResult">Old task's result value.</param>
        /// <returns>>A reference to the newly generated task.</returns>
        public IMyTask<TNewResult> Enqueue<TResult, TNewResult>
            (Func<TResult, TNewResult> func, TResult oldResult)
        {
            var myTask = new MyContinuedTask<TResult, TNewResult>(func, oldResult, this);

            onShutdown += myTask.Abort;
            queue.Enqueue(myTask.Calculate);
            reset.Set();
            return myTask;
        }

        /// <summary>
        /// Shuts all the threads down after they end calculationg their last task.
        /// </summary>
        public void Shutdown()
        {
            cancelTokenSource.Cancel();
            onShutdown();

            reset.Close();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
