using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace UserService.Models
{
    public class DatabaseContext : DbContext
    {
        // Define the constructor
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        // Define the OnConfiguring method
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Define the path for the SQLite database only if not configured by DI container
                const string DatabasePath = "Data Source=/app/UserService/UsersDatabase/UsersDatabase.db";
                optionsBuilder.UseSqlite(DatabasePath);
            }
        }
    
    // Define the DbSet for Users
    public DbSet<Users> Users { get; set; }
    }
    
    // Define the Users class
    public class Users
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Password { get; set; }
    }
}