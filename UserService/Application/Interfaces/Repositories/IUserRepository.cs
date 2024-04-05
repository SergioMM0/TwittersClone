using UserService.Core.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories;

public interface IUserRepository {
    bool CheckUserIdExists(int userId);
    bool CheckUserExists(string username);
    bool CheckUserExists(int id);
    User? CheckPassword(string username, string password);
    User? Create(User user);
    User? GetById(int id);
    List<User> GetAllUsers();
}
