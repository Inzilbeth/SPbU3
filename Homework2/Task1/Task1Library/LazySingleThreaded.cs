using System;

namespace Task1Library
{
    /// <summary>
    /// Class that implements a simple lazy thread-unsafe initialization of an object of type <see cref="{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of an object to be initialized.</typeparam>
    public class LazySingleThreaded<T> : ILazy<T>
    {
        private static T resource;
        private static Func<T> compute;

        /// <summary>
        /// Builds an instance of <see cref="LazySingleThreaded{T}"/> by input function.
        /// </summary>
        /// <param name="func">Function that creates an object of type <see cref="{T}"/>.</param>
        public LazySingleThreaded(Func<T> func)
        {
            compute = func;
        }

        /// <summary>
        /// Grants a simple thread-unsafe access to the instance of type <see cref="{T}"/>
        /// with lazy initialization.
        /// </summary>
        /// <returns>Stored object of type<see cref="{T}"/>.</returns>
        public T Get()
        {
            if (resource == null)
            {
                resource = compute();
            }

            return resource;
        }
    }
}
