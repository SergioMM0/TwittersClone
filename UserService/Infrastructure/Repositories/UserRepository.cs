﻿using UserService.Application.Interfaces.Repositories;
using UserService.Core.Domain.Entities;
using UserService.Models;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository {
    private readonly DatabaseContext _dbContext;

    public UserRepository(DatabaseContext context) {
        _dbContext = context;
    }


    public bool CheckUserIdExists(int userId)
    {
        Console.WriteLine("Checking user id in database...");
        return _dbContext.UsersTable.Any(u => u.Id == userId);
    }

    public bool CheckUserExists(string username)
    {
        Console.WriteLine("Checking username in database...");
        return _dbContext.UsersTable.Any(u => u.Username == username);
    }

    public bool CheckUserExists(int id) {
        Console.WriteLine("Checking id from user in database...");
        return _dbContext.UsersTable.Any(u => u.Id == id);
    }

    public User? CheckPassword(string username, string password) {
        Console.WriteLine("Checking password in database...");
        return _dbContext.UsersTable
            .FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public User? Create(User user) {
        try {
            Console.WriteLine("Creating user in database...");
            var result = _dbContext.UsersTable.Add(user);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public User? GetById(int id) {
        Console.WriteLine("Getting user by id...");
        return _dbContext.UsersTable.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAllUsers() {
        Console.WriteLine("Getting all users...");
        return _dbContext.UsersTable.ToList();
    }
}
