using System;

namespace Task1Library
{
    /// <summary>
    /// Class that implements a simple lazy thread-unsafe initialization of an object of type <see cref="{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of an object to be initialized.</typeparam>
    public class LazySingleThreaded<T> : ILazy<T>
    {
        private T instance;
        private Func<T> supplier;

        /// <summary>
        /// Checks if an instance is already initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Builds an instance of <see cref="LazySingleThreaded{T}"/> by input function.
        /// </summary>
        /// <param name="supplier">Function that creates an object of type <see cref="{T}"/>.</param>
        public LazySingleThreaded(Func<T> supplier)
        {
            this.supplier = supplier;
            IsInitialized = false;
        }

        /// <summary>
        /// Grants a simple thread-unsafe access to the instance of type <see cref="{T}"/>
        /// with lazy initialization.
        /// </summary>
        /// <returns>Stored object of type<see cref="{T}"/>.</returns>
        public T Get()
        {
            if (!IsInitialized)
            {
                instance = supplier();
                IsInitialized = true;
                supplier = null;
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
            if (obj is LazySingleThreaded<T> && obj != null)
            {
                var that = obj as LazySingleThreaded<T>;

                return instance.Equals(that.instance) &&
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
                supplier.GetHashCode();

            return hashCode;
        }
    }
}
