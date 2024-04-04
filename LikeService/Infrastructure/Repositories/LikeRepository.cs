using LikeService.Core.Entities;
using LikeService.Infrastructure.Context;

namespace LikeService.Infrastructure.Repositories;

public class LikeRepository{
    private readonly DatabaseContext _dbContext;

    public LikeRepository(DatabaseContext context) {
        _dbContext = context;
    }
    
    public Like? Add(Like like) {
        try {
            Console.WriteLine("Creating like in database...");
            _dbContext.LikesTable.Add(like);
            _dbContext.SaveChanges();
            return like;
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    
    public Like? GetById(int likeId) {
        try {
            Console.WriteLine("Getting like from database...");
            return _dbContext.LikesTable.FirstOrDefault(p => p.Id == likeId);
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    
    public void Remove(Like like) {
        try {
            Console.WriteLine("Deleting like from database...");
            _dbContext.LikesTable.Remove(like);
            _dbContext.SaveChanges();
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}