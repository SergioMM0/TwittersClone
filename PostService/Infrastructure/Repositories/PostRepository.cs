using PostService.Core.Entities;
using PostService.Infrastructure.Context;

namespace PostService.Infrastructure.Repositories;

public class PostRepository {
    private readonly DatabaseContext _dbContext;

    public PostRepository(DatabaseContext context) {
        _dbContext = context;
    }
    
    public async Task<Post?> CreateAsync(Post post) {
        try {
            Console.WriteLine("Creating post in database...");
            _dbContext.PostsTable.Attach(post);
            await _dbContext.SaveChangesAsync();
            return post;
        } catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}