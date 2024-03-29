using Microsoft.EntityFrameworkCore;
using UserService.Core.Domain.Entities;
using UserService.Models;

namespace UserService.Infrastructure.Repositories;

public class UserRepository {
    private readonly DatabaseContext _DbContext;

    public UserRepository(DatabaseContext context) {
        _DbContext = context;
    }

    public async Task<bool> CheckUserExists(string username) {
        return await _DbContext.Users.AnyAsync(u => u.Name == username);
    }

    public async Task<Users?> CheckPassword(string username, string password) {
        return await _DbContext.Users
            .FirstOrDefaultAsync(u => u.Name == username && u.Password == password);
    }
}
