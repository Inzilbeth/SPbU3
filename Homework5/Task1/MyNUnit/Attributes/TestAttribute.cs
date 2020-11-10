using System;

namespace MyNUnitLib
{
    /// <summary>
    /// Attribute for marking test methods.
    /// </summary>
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Expected exception type.
        /// </summary>
        public Type ExpectedException { get; private set; }

        /// <summary>
        /// Massage reasoning the ignorance of the test.
        /// </summary>
        public string IgnoreMessage { get; private set; }

        /// <summary>
        /// Whether the test should be ignored.
        /// </summary>
        public bool IsIgnored
            => IgnoreMessage != "";

        /// <summary>
        /// Applies an attribute with input parameters.
        /// </summary>
        /// <param name="ignore">Whether test should be ignored.</param>
        /// <param name="expected">Expected exception type.</param>
        public TestAttribute(string ignore = "", Type expected = null)
        {
            ExpectedException = expected;
            IgnoreMessage = ignore;
        }
    }
}
