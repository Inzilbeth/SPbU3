using System;

namespace Task1Library
{
    /// <summary>
    /// Class that implements a lazy thread-safe initialization of an object of type <see cref="{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of an object to be initialized.</typeparam>
    public class LazyMultiThreaded<T> : ILazy<T>
    {
        private T resource;
        private volatile bool isInitialized;
        private Func<T> compute;
        private readonly object syncRoot = new object();

        /// <summary>
        /// Builds an instance of <see cref="LazyMultiThreaded{T}"/> by input function.
        /// </summary>
        /// <param name="func">Function that creates an object of type <see cref="{T}"/>.</param>
        public LazyMultiThreaded(Func<T> func)
        {
            compute = func;
            isInitialized = false;
        }

        /// <summary>
        /// Grants a thread-safe access to the instance of type <see cref="{T}"/>
        /// with lazy initialization using double checked locking.
        /// </summary>
        /// <returns>Stored object of type<see cref="{T}"/>.</returns>
        public T Get()
        {
            if (!isInitialized)
            {
                lock (syncRoot)
                {
                    if (!isInitialized)
                    {
                        resource = compute();
                        isInitialized = true;
                        compute = null;
                    }
                }
            }

            return resource;
        }
    }
}
