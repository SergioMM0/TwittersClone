using Microsoft.EntityFrameworkCore;
using FollowersService.Core.Domain.Entities;

namespace FollowersService.Models;

public class DatabaseContext : DbContext
{
    public DbSet<Follower> FollowersTable { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            "Data Source=./FollowersDatabase/db.db"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follower>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}
