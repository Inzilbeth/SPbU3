using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    /// <summary>
    /// Model with info about tests launched from a single uploaded set of assemblies.
    /// </summary>
    public class TestRun
    {
        /// <summary>
        /// Shows when tests started running.
        /// </summary>
        [Key]
        public DateTime LaunchedAt { get; set; }

        /// <summary>
        /// Tested assemblies.
        /// </summary>
        public virtual ICollection<AssemblyFile> Source { get; set; }

        /// <summary>
        /// Results of testing.
        /// </summary>
        public virtual ICollection<TestInfoModel> Report { get; set; }
    }
}
