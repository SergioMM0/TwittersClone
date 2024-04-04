using PostService.Core.Entities;
using PostService.Infrastructure.Context;

namespace PostService.Infrastructure.Repositories;

public class PostRepository {
    private readonly DatabaseContext _dbContext;

    public PostRepository(DatabaseContext context) {
        _dbContext = context;
    }
    
    public Post? Attach(Post post) {
        try {
            Console.WriteLine("Creating post in database...");
            _dbContext.PostsTable.Add(post);
            _dbContext.SaveChanges();
            return post;
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}