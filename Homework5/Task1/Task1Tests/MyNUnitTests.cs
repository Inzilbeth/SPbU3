using MyNUnitLib;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TestAttribute = NUnit.Framework.TestAttribute;

namespace Tests
{
    [TestFixture]
    public class MyNUnitTests
    {
        private List<TestInfo> TestsResults;
        private List<string> expectedResultsMethods;
        private const string path = "..\\..\\..\\..\\Test1";

        [SetUp]
        public void Setup()
        {
            TestsResults = new List<TestInfo>();
            expectedResultsMethods = new List<string>();
            expectedResultsMethods.Add("SuccessfulMethod");
            expectedResultsMethods.Add("IgnoreMethod");
            expectedResultsMethods.Add("IgnoreMethodThrowingException");
            expectedResultsMethods.Add("ExpectedExceptionThrown");
            expectedResultsMethods.Add("ExceptionOnFail");
            expectedResultsMethods.Add("UnexpectedExceptionThrown");
        }

        [Test]
        public void CorrectMethodsAreTestedTest()
        {
            var resultsTestPath = path;
            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var names = new List<string>();

            foreach (var res in TestsResults)
            {
                names.Add(res.MethodName);
            }

            Assert.AreEqual(names.Intersect(expectedResultsMethods).Count(), expectedResultsMethods.Count);
        }

        [Test]
        public void RegularTestPassedTest()
        {
            var resultsTestPath = path;

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var successInfo = TestsResults.Find(i => i.MethodName == "SuccessfulMethod");

            Assert.IsTrue(successInfo.IsSuccessful);
        }

        [Test]
        public void IgnoreTest()
        {
            var resultsTestPath = path;

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var ignoreInfo = TestsResults.Find(i => i.MethodName == "IgnoreMethod");
            var exceptionIgnoreInfo = TestsResults.Find(i => i.MethodName == "IgnoreMethodThrowingException");

            Assert.IsTrue(ignoreInfo.IsIgnored);
            Assert.AreEqual("Some reason to ignore this test.", ignoreInfo.IgnoranceReason);
            Assert.IsTrue(exceptionIgnoreInfo.IsIgnored);
        }

        [Test]
        public void ExpectedExceptionTest()
        {
            var resultsTestPath = path;

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var expectedInfo = TestsResults.Find(i => i.MethodName == "ExpectedExceptionThrown");

            Assert.AreEqual(expectedInfo.ExpectedException, expectedInfo.ActualException);
            Assert.IsTrue(expectedInfo.IsSuccessful);
        }

        [Test]
        public void FailExceptionTest()
        {
            var resultsTestPath = path;

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var failInfo = TestsResults.Find(i => i.MethodName == "ExceptionOnFail");

            Assert.AreEqual(null, failInfo.ExpectedException);
            Assert.AreNotEqual(null, failInfo.ActualException);
            Assert.IsFalse(failInfo.IsSuccessful);
        }

        [Test]
        public void UnexpectedExceptionTest()
        {
            var resultsTestPath = path;

            var regularTestsReport = MyNUnit.RunTestsAndGetReport(resultsTestPath);

            foreach (var list in regularTestsReport.Values)
            {
                foreach (var info in list)
                {
                    TestsResults.Add(info);
                }
            }

            var exceptionInfo = TestsResults.Find(i => i.MethodName == "UnexpectedExceptionThrown");

            Assert.AreNotEqual(exceptionInfo.ActualException, exceptionInfo.ExpectedException);
            Assert.IsFalse(exceptionInfo.IsSuccessful);
        }
    }
}
