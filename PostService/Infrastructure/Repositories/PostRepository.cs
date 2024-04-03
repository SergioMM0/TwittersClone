using PostService.Core.Entities;
using PostService.Infrastructure.Context;

namespace PostService.Infrastructure.Repositories;

public class PostRepository {
    private readonly DatabaseContext _dbContext;

    public PostRepository(DatabaseContext context) {
        _dbContext = context;
    }
    
    public Post? Create(Post post) {
        try {
            Console.WriteLine("Creating post in database...");
            var result = _dbContext.PostsTable.Add(post);
            _dbContext.SaveChanges();
            return result.Entity;
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}