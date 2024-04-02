using Microsoft.EntityFrameworkCore;
using UserService.Core.Domain.Entities;

namespace UserService.Models;

public class DatabaseContext : DbContext {
    public DbSet<User> UsersTable { get; set; }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {

    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite(
            "Data source=./db.db"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}
