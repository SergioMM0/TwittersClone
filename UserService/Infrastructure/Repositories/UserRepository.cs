using UserService.Core.Domain.Entities;
using UserService.Infrastructure.Context;

namespace UserService.Infrastructure.Repositories;

public class UserRepository {
    private readonly UserDbContext _dbContext;
    public UserRepository(UserDbContext context) {
        _dbContext = context;
    }
    public bool CheckUserExists(string username) {
        return _dbContext.Users.SingleOrDefault(user => user.Username == username) is not null;
    }
    public User? CheckPassword(string username, string password) {
        return _dbContext.Users.SingleOrDefault(user => user.Username == username && user.Password == password);
    }
}
