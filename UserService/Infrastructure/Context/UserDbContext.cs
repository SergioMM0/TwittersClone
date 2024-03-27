using Microsoft.EntityFrameworkCore;
using UserService.Core.Domain.Entities;

namespace UserService.Infrastructure.Context;

public class UserDbContext : DbContext{
    public DbSet<User> Users { get; set; }
    
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) {
    }

        
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(new User {
            Id = Guid.NewGuid(),
            Username = "test",
            Password = "test"
        });
    }
}
