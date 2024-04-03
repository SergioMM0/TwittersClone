using PostService.Core.Entities;

namespace PostService.Core.Services;

public class PostManager
{
    public void CreatePost(string body, string author)
    {
        Console.WriteLine($"Author: {author},created a post with body: {body}");
    }

    public void UpdatePost(string body, string author, int id)
    {
        Console.WriteLine($"{author} has successfully updated post with id: {id}. New body: {body}");
    }

    public void DeletePost(int id)
    {
        Console.WriteLine($"Deleted post with id: {id}");
    }

    public Post? GetPost(int id)
    {
        switch (id)
        {
            case 1:
                Console.WriteLine($"Retrieving post with id: {id}");
                return new Post()
                {
                    Body = $"Post number {id}",
                    Author = "Sergio"
                };
            case 2:
                Console.WriteLine($"Retrieving post with id: {id}");
                return new Post()
                {
                    Body = $"Post number {id}",
                    Author = "Alex"
                };
            default:
                Console.WriteLine("Post not found");
                return null;
        }
    }

    public List<Post> GetPosts()
    {
        Console.WriteLine("Retrieving all posts");
        return new List<Post>() {
            new Post() {
                Body = "Post number 1",
                Author = "Sergio"},
            new Post() {
                Body = "Post number 2",
                Author = "Alex"}
        };
    }
}