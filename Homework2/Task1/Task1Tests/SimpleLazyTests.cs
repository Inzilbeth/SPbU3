using NUnit.Framework;
using Task1Library;

namespace Task1Tests
{
    [TestFixture(typeof(LazySingleThreaded<int>))]
    [TestFixture(typeof(LazyMultiThreaded<int>))]
    public class SimpleLazyTests<TLazy>
    {
        private ILazy<int> lazy;
        private int count;

        [SetUp]
        public void Setup()
        {
            count = 0;

            if (typeof(TLazy) == typeof(LazySingleThreaded<int>))
            {
                lazy = LazyFactory.CreateSingleThreaded(() => { ++count; return 1; });
            }
            else
            {
                lazy = LazyFactory.CreateMultiThreaded(() => { ++count; return 1; });
            }
        }

        [Test]
        public void IsNotInitializedBeforeCall()
            => Assert.IsFalse(lazy.IsInitialized);

        [Test]
        public void ReturnsCorrectValue()
            => Assert.AreEqual(1, lazy.Get());
        
        [Test]
        public void IsInitializedAfterCall()
        {
            lazy.Get();

            Assert.IsTrue(lazy.IsInitialized);
        }

        [Test]
        public void DoesNotComputeMoreThanOnce()
        {
            lazy.Get();
            lazy.Get();
            lazy.Get();

            Assert.AreEqual(1, count);
        }
    }
}
