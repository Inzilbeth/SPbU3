using Microsoft.EntityFrameworkCore;

namespace Task1.Models
{
    public class Repository : DbContext
    {
        public virtual DbSet<TestRun> Runs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;"
                + @"Database=Task1;Trusted_Connection=True;");
        }
    }
}
