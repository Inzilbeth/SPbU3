using System;

namespace Task1Library
{
    /// <summary>
    /// Factory that can create classes that implement <see cref="ILazy{T}"/> interface.
    /// </summary>
    public class LazyFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="LazySingleThreaded{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of an object for Lazy initialiation.</typeparam>
        /// <param name="supplier">Function that computes an object of type <see cref="{T}"/>.</param>
        /// <returns><see cref="LazySingleThreaded{T}"/> instance.</returns>
        public static LazySingleThreaded<T> CreateSingleThreaded<T>(Func<T> supplier)
        {
            return new LazySingleThreaded<T>(supplier);
        }

        /// <summary>
        /// Creates an instance of <see cref="LazyMultiThreaded{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of an object for Lazy initialiation.</typeparam>
        /// <param name="supplier">Function that computes an object of type <see cref="{T}"/>.</param>
        /// <returns><see cref="LazyMultiThreaded{T}"/> instance.</returns>
        public static LazyMultiThreaded<T> CreateMultiThreaded<T>(Func<T> supplier)
        {
            return new LazyMultiThreaded<T>(supplier);
        }
    }
}
