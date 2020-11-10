using System;

namespace MyNUnitLib
{
    /// <summary>
    /// Represents method's test info.
    /// </summary>
    public class TestInfo
    {
        /// <summary>
        /// Name of a method.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Exception that is expected.
        /// </summary>
        public Type ExpectedException { get; private set; }

        /// <summary>
        /// Exception that was thrown during testing.
        /// </summary>
        public Type ActualException { get; private set; }

        /// <summary>
        /// Whether a test method should be ignored.
        /// </summary>
        public bool IsIgnored { get; private set; }

        /// <summary>
        /// Whether a test was successfull.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// If the test is ignored, this string explains why. Otherwise is empty.
        /// </summary>
        public string IgnoranceReason { get; private set; }

        /// <summary>
        /// Amount of time testing has taken.
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Constructor for ignored test methods.
        /// </summary>
        public TestInfo(string name, string ignoranceReason)
        {
            IsIgnored = true;

            MethodName = name;
            IgnoranceReason = ignoranceReason;
        }

        /// <summary>
        /// Constructor for unignored test methods.
        /// </summary>
        public TestInfo(string name, bool isSuccessful, Type expectedException, Type actualException, TimeSpan time)
        {
            IsIgnored = false;
            IgnoranceReason = "";

            MethodName = name;
            IsSuccessful = isSuccessful;
            ExpectedException = expectedException;
            ActualException = actualException;
            Time = time;
        }
    }
}