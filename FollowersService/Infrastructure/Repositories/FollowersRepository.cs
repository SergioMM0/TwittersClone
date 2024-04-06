using FollowersService.Application.Interfaces.Repositories;
using FollowersService.Core.Domain.Entities;
using FollowersService.Models;

namespace FollowersService.Infrastructure.Repositories;

public class FollowersRepository : IFollowersRepository
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

    public Follower? Create(Follower follower)
    {
        // Check if the follower combination already exists
        bool followerExists = _dbContext.FollowersTable.Any(f => f.UserId == follower.UserId && f.FollowerId == follower.FollowerId);
        if (followerExists)
        {
            Console.WriteLine("This follower combination already exists in the database.");
            return null; // Or throw a custom exception
        }

        try
        {
            Console.WriteLine("Creating follower in database...");
            var result = _dbContext.FollowersTable.Add(follower);
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
            .Distinct()
            .ToList();
    }

    public bool DeleteFollower(int userId, int followerId)
    {
        try
        {
            Console.WriteLine("Deleting follower from database...");
            var followers = _dbContext.FollowersTable.Where(f => f.UserId == userId && f.FollowerId == followerId).ToList();
        
            if (!followers.Any())
            {
                Console.WriteLine("No matching follower found to delete.");
                return false;
            }
        
            _dbContext.FollowersTable.RemoveRange(followers);
            _dbContext.SaveChanges();
            Console.WriteLine("Follower(s) deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
    public bool ToggleNotification(int userId, int followerId, out bool newState)
    {
        newState = false; // Default value
        try
        {
            var follower = _dbContext.FollowersTable.FirstOrDefault(f => f.UserId == userId && f.FollowerId == followerId);
            if (follower is null)
            {
                return false;
            }
        
            follower.ListenToNotifications = !follower.ListenToNotifications;
            newState = follower.ListenToNotifications; // Set the out parameter to the new state
        
            _dbContext.SaveChanges();
            return true; // Indicate success
        }
        catch (Exception)
        {
            return false; // Indicate failure
        }
    }

    public List<int> GetNotifListeners(int userId)
    {
        return _dbContext.FollowersTable
            .Where(f => f.UserId == userId && f.ListenToNotifications == true) // Filter by the user ID and notification status
            .Select(f => f.FollowerId) // Select the follower IDs
            .Distinct() // Remove duplicates
            .ToList(); // Return the list
    }
}
