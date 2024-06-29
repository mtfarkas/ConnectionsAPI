using ConnectionsAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConnectionsAPI.Database
{
    public class ConnectionsContext(DbContextOptions<ConnectionsContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Puzzle> Puzzles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Puzzle>()
                .HasIndex(x => x.PrintDate).IsUnique();
            modelBuilder.Entity<Puzzle>()
                .Ignore(x => x.NextPrintDate);
            modelBuilder.Entity<Puzzle>()
                .Ignore(x => x.PrevPrintDate);


            base.OnModelCreating(modelBuilder);
        }
    }
}
