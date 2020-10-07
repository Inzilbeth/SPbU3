using System;
using System.Threading;

namespace Task1
{
    /// <summary>
    /// Represents a task that is generated from another task.
    /// </summary>
    /// <typeparam name="TOldResult">Old task's result type.</typeparam>
    /// <typeparam name="TResult">New task's result type.</typeparam>
    public class MyContinuedTask<TOldResult, TResult> : IMyTask<TResult>
    {
        private Func<TOldResult, TResult> function;
        private TResult result;
        private TOldResult oldResult;
        private bool isPoolStopped;

        private ManualResetEvent reset;
        private MyThreadPool creator;

        /// <summary>
        /// Checks whether the result is already calculated.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Creates a new <see cref="MyContinuedTask{TOldResult, TResult}"/> instance.
        /// </summary>
        /// <param name="function">Function to calculate the value.</param>
        /// <param name="oldResult">Old task's result.</param>
        /// <param name="creator">Thread pool that created the task.</param>
        public MyContinuedTask(
            Func<TOldResult, TResult> function, TOldResult oldResult, MyThreadPool creator)
        {
            if (function == null || creator == null || oldResult == null)
            {
                throw new ArgumentNullException();
            }

            this.creator = creator;
            this.oldResult = oldResult;
            this.function = function;
            
            isPoolStopped = false;
            IsCompleted = false;
            
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
        /// Calculates the result.
        /// </summary>
        public void Calculate()
        {
            try
            {
                result = function(oldResult);
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
