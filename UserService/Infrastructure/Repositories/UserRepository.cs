using UserService.Core.Domain.Entities;

namespace UserService.Infrastructure.Repositories;

public class UserRepository {
    public bool CheckUserExists(string username) {
        return username == "test";
    }
    public User? CheckPassword(string username, string password) {
        return new User {
            Username = username,
            Password = password
        };
    }
}
