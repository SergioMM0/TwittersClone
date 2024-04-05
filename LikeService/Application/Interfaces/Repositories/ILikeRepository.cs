using LikeService.Core.Entities;

namespace LikeService.Application.Interfaces.Repositories;

public interface ILikeRepository {
    Like? Add(Like like);
    Like? GetById(int likeId);
    void Remove(Like like);
}