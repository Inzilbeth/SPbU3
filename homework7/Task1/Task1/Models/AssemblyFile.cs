using System;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    public class AssemblyFile
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }
    }
}
