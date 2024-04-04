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

    public bool FollowerExists(int userId, int followerId)
    {
        return _dbContext.FollowersTable.Any(f => f.UserId == userId && f.FollowerId == followerId);
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

    public List<int> GetFollowers(int userId)
    {
        return _dbContext.FollowersTable
            .Where(f => f.UserId == userId)
            .Select(f => f.FollowerId)
            .ToList();
    }

    public bool DeleteFollower(int userId, int followerId)
    {
        try
        {
            Console.WriteLine("Deleting follower from database...");
            var follower = _dbContext.FollowersTable.FirstOrDefault(f => f.UserId == userId && f.FollowerId == followerId);
            if (follower is null)
            {
                return false;
            }
            _dbContext.FollowersTable.Remove(follower);
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

}
