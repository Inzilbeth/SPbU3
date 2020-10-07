using System;
using System.Threading;

namespace Task1
{
    /// <summary>
    /// A task which result calculation is handled by the thread pool.
    /// </summary>
    /// <typeparam name="TResult">Type of result value.</typeparam>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> function;
        private TResult result;
        private bool isPoolStopped;
        
        private ManualResetEvent reset;
        private MyThreadPool creator;

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
        public MyTask(Func<TResult> function, MyThreadPool creator)
        {
            if (function == null || creator == null)
            {
                throw new ArgumentNullException();
            }

            this.function = function;
            this.creator = creator;
            IsCompleted = false;
            isPoolStopped = false;

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
                    
                    if (isPoolStopped)
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
                reset.Set();
            }
            catch (Exception exception)
            {
                throw new AggregateException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Aborts the task.
        /// </summary>
        public void Abort()
        {
            reset.Set();
            isPoolStopped = true;
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

            var result = Result;
            return creator.Enqueue(func, result);
        }
    }
}
