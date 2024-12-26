using ConnectionsAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConnectionsAPI.Database;
public class ConnectionsContext(DbContextOptions<ConnectionsContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public required DbSet<ConnectionsPuzzle> ConnectionsPuzzles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConnectionsPuzzle>()
            .HasIndex(x => x.PrintDate).IsUnique();
        modelBuilder.Entity<ConnectionsPuzzle>()
            .Ignore(x => x.NextPrintDate);
        modelBuilder.Entity<ConnectionsPuzzle>()
            .Ignore(x => x.PrevPrintDate);


        base.OnModelCreating(modelBuilder);
    }
}

