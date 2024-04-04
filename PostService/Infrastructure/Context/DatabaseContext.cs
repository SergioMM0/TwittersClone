using Microsoft.EntityFrameworkCore;
using PostService.Core.Entities;

namespace PostService.Infrastructure.Context;

public class DatabaseContext : DbContext {
    public DbSet<Post> PostsTable { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite(
            "Data Source=./PostDatabase/db.db"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Post>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}