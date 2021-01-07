using System;
using System.ComponentModel.DataAnnotations;
using MyNUnitLib;

namespace Task1.Models
{
    /// <summary>
    /// Simplified model of <see cref="TestInfo"/>.
    /// </summary>
    public class TestInfoModel
    {
        /// <summary>
        /// Id for the database.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of a method.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Exception that is expected.
        /// </summary>
        public string ExpectedException { get; set; }

        /// <summary>
        /// Exception that was thrown during testing.
        /// </summary>
        public string ActualException { get; set; }

        /// <summary>
        /// Whether a test method should be ignored.
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// Whether a test was successfull.
        /// </summary>
        public bool IsSuccessful { get;  set; }

        /// <summary>
        /// If the test is ignored, this string explains why. Otherwise is empty.
        /// </summary>
        public string ReasonToIgnore { get; set; }

        /// <summary>
        /// Amount of time testing has taken.
        /// </summary>
        public string Time { get; set; }
    }
}
