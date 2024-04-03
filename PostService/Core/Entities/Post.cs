namespace PostService.Core.Entities;

public class Post {
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Body { get; set; }

    public required int AuthorId { get; set; }

    public List<int>? CommentsId { get; set; }
}
