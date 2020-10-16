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
        /// <summary>
        /// Represents inner functionality of a task.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private class InnerTask<TResult>
        {
            private Func<TResult> function;
            private TResult result;
            private bool isPoolAborted;
            private Exception exception;

            private ManualResetEvent reset;
            private MyThreadPool creator;

            private object sync = new object();

            /// <summary>
            /// Checks whether the result is already calculated.
            /// </summary>
            public bool IsCompleted { get; private set; }

            /// <summary>
            /// Creates a new <see cref="MyTask{TResult}"/> instance with
            /// specified parameters.
            /// </summary>
            /// <param name="function">Function to calculate the result value.</param>
            /// <param name="creator">Thread pool that created the task.</param>
            public InnerTask(Func<TResult> function, MyThreadPool creator)
            {
                if (function == null || creator == null)
                {
                    throw new ArgumentNullException();
                }

                this.function = function;
                this.creator = creator;
                IsCompleted = false;
                isPoolAborted = false;

                reset = new ManualResetEvent(false);
            }

            /// <summary>
            /// Returns the result value, blocks calling thread
            /// if it's not calculated untill it is.
            /// </summary>
            public TResult Result
            {
                get
                {
                    if (!IsCompleted)
                    {
                        reset.WaitOne();

                        if (exception != null)
                        {
                            throw new AggregateException(exception);
                        }

                        if (isPoolAborted)
                        {
                            throw new InvalidOperationException("Task was aborted.");
                        }

                        return result;
                    }

                    return result;
                }
            }

            /// <summary>
            /// Calculates the result value.
            /// </summary>
            public void Calculate()
            {
                try
                {
                    result = function();
                    IsCompleted = true;
                    function = null;
                    reset.Set();
                }
                catch (Exception exception)
                {
                    this.exception = exception;
                }
            }

            /// <summary>
            /// Aborts the task.
            /// </summary>
            public void Abort()
            {
                isPoolAborted = true;
                reset.Set();
            }

            /// <summary>
            /// Generates a new task based on this task's result.
            /// </summary>
            /// <typeparam name="TNewResult">Type of the new task's result.</typeparam>
            /// <param name="func">Function to calculate new value.</param>
            /// <returns>New task.</returns>
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                if (func == null)
                {
                    throw new ArgumentNullException();
                }

                return creator.Enqueue(() => func(Result));
            }
        }

        /// <summary>
        /// Represents a task returned to the user.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private InnerTask<TResult> task;
            
            public MyTask(InnerTask<TResult> task)
            {
                this.task = task;
            }

            public bool IsCompleted => task.IsCompleted;

            public TResult Result => task.Result;

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                return task.ContinueWith(func);
            }
        }
        
        private Thread[] threads;
        private ConcurrentQueue<Action> queue;
        private ManualResetEvent reset;
        private CancellationTokenSource cancelTokenSource;

        public event Action OnShutdown;
        
        private object locker = new object();
        
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
                threads[i] = new Thread(() =>
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
                });

                threads[i].Name = $"WT #{i}";
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
            if (!cancelTokenSource.IsCancellationRequested)
            {
                lock (locker)
                {
                    if (!cancelTokenSource.IsCancellationRequested)
                    {
                        var inner = new InnerTask<TResult>(func, this);

                        OnShutdown += inner.Abort;
                        queue.Enqueue(inner.Calculate);
                        reset.Set();
                        return new MyTask<TResult>(inner);
                    }
                }
            }

            throw new InvalidOperationException("Pool is stopped");
        }

        /// <summary>
        /// Shuts all the threads down after they end calculationg their last task.
        /// </summary>
        public void Shutdown()
        {
            lock (locker)
            {
                cancelTokenSource.Cancel();
            }
            
            OnShutdown();

            reset.Close();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
