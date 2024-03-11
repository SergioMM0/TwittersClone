using System;
using Microsoft.EntityFrameworkCore;

namespace UserService.Models
{
    public class DatabaseContext : DbContext
    {
        // Define the path for the SQLite database
        private const string DatabasePath = "Data Source=UserService/UsersDatabase.db";

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // DbSet for your Users entity
        public DbSet<Users> Users { get; set; }

        // Override OnConfiguring method to configure SQLite connection
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQLite with the specified database path
            optionsBuilder.UseSqlite(DatabasePath);
        }
    }

    public class Users
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}