using UserService.Core.Domain.Entities;

namespace UserService.Infrastructure.Repositories;

public class UserRepository {
    public bool CheckUserExists(string username) {
        return username is "test" or "admin";
    }
    public User? CheckPassword(string username, string password) {
        if (username == "admin")
            return new User {
                Username = username,
                Password = password
            };
        return null;
    }
}
