using UserService.Core.Domain.Entities;
using UserService.Models;

namespace UserService.Infrastructure.Repositories;

public class UserRepository {
    private readonly DatabaseContext _DbContext;

    public UserRepository(DatabaseContext context) {
        _DbContext = context;
    }

    public bool CheckUserExists(string username) {
        Console.WriteLine("Checking username in database...");
        return _DbContext.UsersTable.Any(u => u.Username == username);
    }

    public User? CheckPassword(string username, string password) {
        Console.WriteLine("Checking password in database...");
        return _DbContext.UsersTable
            .FirstOrDefault(u => u.Username == username && u.Password == password);
    }
    
    public void Create(User user) {
        Console.WriteLine("Creating user in database...");
        _DbContext.UsersTable.Add(user);
        _DbContext.SaveChanges();
    }
}
