using System;

namespace Task1Library
{
    public class LazySingleThreaded<T> : ILazy<T>
    {
        public T Get()
        {
            throw new NotImplementedException();
        }
    }
}
