using LikeService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LikeService.Infrastructure.Context;

public class DatabaseContext : DbContext
{
    public DbSet<Like> LikeTable { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            "Data Source=./LikeDatabase/db.db"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Like>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}