using PostService.Core.Entities;

namespace PostService.Application.Interfaces.Repositories;

public interface IPostRepository {
    Post? Add(Post post);
    Post? GetById(int postId);
    void Delete(Post post);
    List<Post> GetAll();
}
