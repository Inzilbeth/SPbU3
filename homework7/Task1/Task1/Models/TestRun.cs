using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    public class TestRun
    {
        [Key]
        public Guid Id { get; set; }

        public virtual ICollection<AssemblyFile> Source { get; set; }

        public virtual ICollection<TestInfoModel> Report { get; set; }
    }
}
