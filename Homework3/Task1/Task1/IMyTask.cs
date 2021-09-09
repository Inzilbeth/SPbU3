using System;

namespace Task1
{
    /// <summary>
    /// A task which result calculation is handled by the thread pool.
    /// </summary>
    /// <typeparam name="TResult">Type of result value.</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Checks whether the result is already calculated.
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// Returns the result value, blocks calling thread
        /// if it's not calculated untill it is.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Generates a new task based on a function and 
        /// original task value.
        /// </summary>
        /// <typeparam name="TNewResult">New result value type.</typeparam>
        /// <param name="func">Function used to calculate new result value.</param>
        /// <returns>New task.</returns>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}
