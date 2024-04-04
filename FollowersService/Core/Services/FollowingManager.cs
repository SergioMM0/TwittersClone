using FollowersService.Application.Clients;
using FollowersService.Core.Domain.Entities;
using FollowersService.Infrastructure.Repositories;
using RabbitMQMessages.Follow;

namespace FollowersService.Core.Services; 
public class FollowingManager {
    private readonly MessageClient _messageClient;
    private readonly FollowersRepository _followersRepository;

    public FollowingManager(MessageClient messageClient, FollowersRepository followersRepository) {
        _messageClient = messageClient;
        _followersRepository = followersRepository;
    }

    public void AddFollower(int userId, int followerId) {
        // Check to prevent adding the same follower more than once
        if (_followersRepository.FollowerExists(userId, followerId)) {
            Console.WriteLine($"Follower {followerId} already added to user {userId}.");
            _messageClient.Send(new AddFollowerMsg() {
                UserId = userId,
                FollowerId = followerId,
                FollowerAdded = false
            }, "API/follower-added-response");
        return;
        }
        
        Console.WriteLine("Adding follower with ID: " + followerId + " to user with ID: " + userId);
        var follower = new Follower {
                UserId = userId,
                FollowerId = followerId
        };
        var result = _followersRepository.Create(follower);

        if (result is null) {
            Console.WriteLine("Follower creation failed... sending response to API");
            _messageClient.Send(new AddFollowerMsg() {
                UserId = userId,
                FollowerId = followerId,
                FollowerAdded = false
            }, "API/follower-added-response");
        } else {
            Console.WriteLine("Follower added successfully... sending response to API");
            _messageClient.Send(new AddFollowerMsg() {
                UserId = userId,
                FollowerId = followerId,
                FollowerAdded = true
            }, "API/follower-added-response");
        }
    }

    public void FetchFollowers(int userId) {
        var followers = _followersRepository.GetFollowers(userId);
        Console.WriteLine("Fetching followers for user with ID: " + userId);
        _messageClient.Send(new FetchFollowersMsg() {
            UserId = userId,
            FollowerIds = new List<int>(followers)
        }, "API/fetch-followers-response");
    }
}
