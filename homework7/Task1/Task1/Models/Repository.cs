using Microsoft.EntityFrameworkCore;

namespace Task1.Models
{
    /// <summary>
    /// Repository for accessing the database.
    /// </summary>
    public class Repository : DbContext
    {
        /// <summary>
        /// All recorded test runs.
        /// </summary>
        public virtual DbSet<TestRun> Runs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;"
                + @"Database=Task1;Trusted_Connection=True;");
        }
    }
}
