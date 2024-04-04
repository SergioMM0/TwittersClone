using PostService.Core.Entities;
using PostService.Infrastructure.Context;

namespace PostService.Infrastructure.Repositories;

public class PostRepository {
    private readonly DatabaseContext _dbContext;

    public PostRepository(DatabaseContext context) {
        _dbContext = context;
    }
    
    public Post? Add(Post post) {
        try {
            Console.WriteLine("Creating post in database...");
            _dbContext.PostsTable.Add(post);
            _dbContext.SaveChanges();
            return post;
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    
    public Post? GetById(int postId) {
        try {
            Console.WriteLine("Getting post from database...");
            return _dbContext.PostsTable.FirstOrDefault(p => p.Id == postId);
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    
    public void Delete(Post post) {
        try {
            Console.WriteLine("Deleting post from database...");
            _dbContext.PostsTable.Remove(post);
            _dbContext.SaveChanges();
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
    
    public List<Post> GetAll() {
        return _dbContext.PostsTable.ToList();
    }
}