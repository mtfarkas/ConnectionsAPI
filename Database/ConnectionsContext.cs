using ConnectionsAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConnectionsAPI.Database
{
    public class ConnectionsContext(DbContextOptions<ConnectionsContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<CategoriesPuzzle> CategoriesPuzzles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoriesPuzzle>()
                .HasIndex(x => x.PrintDate).IsUnique();
            modelBuilder.Entity<CategoriesPuzzle>()
                .Ignore(x => x.NextPrintDate);
            modelBuilder.Entity<CategoriesPuzzle>()
                .Ignore(x => x.PrevPrintDate);


            base.OnModelCreating(modelBuilder);
        }
    }
}
