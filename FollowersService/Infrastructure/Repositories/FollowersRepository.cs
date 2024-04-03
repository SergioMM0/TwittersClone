using FollowersService.Core.Domain.Entities;
using FollowersService.Models;

namespace FollowersService.Infrastructure.Repositories;

public class FollowersRepository
{
    private readonly DatabaseContext _dbContext;

    public FollowersRepository(DatabaseContext context)
    {
        _dbContext = context;
    }

    public Follower? Create(Follower Follower)
    {
        try
        {
            Console.WriteLine("Creating follower in database...");
            var result = _dbContext.FollowersTable.Add(Follower);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

}