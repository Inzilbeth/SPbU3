using NUnit.Framework;
using Task1Library;

namespace Task1Tests
{
    class LazyFactoryTests
    {
        private int dummySupplier()
            => 1;

        [Test]
        public void CreateSingleThreadedLazyWorks()
        {
            Assert.AreEqual(new LazySingleThreaded<int>(dummySupplier),
                LazyFactory.CreateSingleThreaded(dummySupplier));
        }

        [Test]
        public void CreateMultiThreadedLazyWorks()
        {
            Assert.AreEqual(new LazyMultiThreaded<int>(dummySupplier),
                LazyFactory.CreateMultiThreaded(dummySupplier));
        }

    }
}
