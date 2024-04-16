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

            base.OnModelCreating(modelBuilder);
        }
    }
}
