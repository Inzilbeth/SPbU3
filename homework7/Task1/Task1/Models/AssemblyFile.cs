using System;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    /// <summary>
    /// Represents an assembly file with dll extension.
    /// </summary>
    public class AssemblyFile
    {
        /// <summary>
        /// Id for the database.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Name of assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Assembly's content.
        /// </summary>
        public byte[] Content { get; set; }
    }
}
