using FollowersService.Core.Domain.Entities;

namespace FollowersService.Application.Interfaces.Repositories;

public interface IFollowersRepository {
    bool FollowerExists(int userId, int followerId);
    Follower Create(Follower follower);
    List<int> GetFollowers(int userId);
    bool DeleteFollower(int userId, int followerId);
    bool ToggleNotification(int userId, int followerId, out bool newState);
    List<int> GetNotifListeners(int userId);

}
