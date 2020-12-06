using System;
using MyNUnitAttributes;

namespace Test1
{
    public class SimpleTests
    {
        [Test]
        public void SuccessfulMethod() { }

        [Test("Some reason to ignore this test.")]
        public void IgnoreMethod() { }

        [Test("Some reason to ignore this test.")]
        public void IgnoreMethodThrowingException()
            => throw new Exception();

        [Test("", typeof(ArgumentNullException))]
        public void ExpectedExceptionThrown()
            => throw new ArgumentNullException();

        [Test]
        public void ExceptionOnFail()
            => throw new Exception();

        [Test("", typeof(ArgumentNullException))]
        public void UnexpectedExceptionThrown()
            => throw new Exception();
    }
}
