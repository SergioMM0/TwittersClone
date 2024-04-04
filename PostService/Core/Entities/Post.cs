namespace PostService.Core.Entities;

public class Post {
    /// <summary>
    /// Id of the post
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the post
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Body of the post
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Id of the author of the post
    /// </summary>
    public required int AuthorId { get; set; }

    /// <summary>
    /// Represents the parent post of this post
    /// </summary>
    public int ParentId { get; set; }
}
