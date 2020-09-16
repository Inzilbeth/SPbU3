using System;

namespace Task1Library
{
    /// <summary>
    /// Class that implements a lazy thread-safe initialization of an object of type <see cref="{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of an object to be initialized.</typeparam>
    public class LazyMultiThreaded<T> : ILazy<T>
    {
        private T instance;
        private volatile bool isInitialized;
        private Func<T> supplier;
        private readonly object syncRoot = new object();

        /// <summary>
        /// Checks if an instance is already initialized.
        /// </summary>
        public bool IsInitialized => isInitialized;

        /// <summary>
        /// Builds an instance of <see cref="LazyMultiThreaded{T}"/> by input function.
        /// </summary>
        /// <param name="supplier">Function that creates an object of type <see cref="{T}"/>.</param>
        public LazyMultiThreaded(Func<T> supplier)
        {
            this.supplier = supplier;
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
                        instance = supplier();
                        isInitialized = true;
                        supplier = null;
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// NOT thread safe.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if objects are equal, esle - false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is LazyMultiThreaded<T> && obj != null)
            {
                var that = obj as LazyMultiThreaded<T>;

                return instance.Equals(that.instance) &&
                    isInitialized.Equals(that.isInitialized) &&
                    supplier.Equals(that.supplier);
            }

            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// NOT thread safe.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            var hashCode = instance.GetHashCode() ^
                isInitialized.GetHashCode() ^
                supplier.GetHashCode();

            return hashCode;
        }
    }
}
