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
                FollowerId = followerId,
                ListenToNotifications = true
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

    public void DeleteFollower(int userId, int followerId) {
        Console.WriteLine("Deleting follower with ID: " + followerId + " from user with ID: " + userId);
       
        var result = _followersRepository.DeleteFollower(userId, followerId);

        if (result) {
            Console.WriteLine("Follower deleted successfully... sending response to API");
            _messageClient.Send(new DeleteFollowerMsg() {
                UserId = userId,
                FollowerId = followerId,
                FollowerDeleted = true
            }, "API/follower-deleted-response");
        } else {
            Console.WriteLine("Follower deletion failed... sending response to API");
            _messageClient.Send(new DeleteFollowerMsg() {
                UserId = userId,
                FollowerId = followerId,
                FollowerDeleted = false
            }, "API/follower-deleted-response");
        }
    }

    public void ToggleNotification(int userId, int followerId) {
    var result = _followersRepository.ToggleNotification(userId, followerId, out bool newState);
    if (result) {
        _messageClient.Send(new ToggleNotificationMsg() {
            UserId = userId,
            FollowerId = followerId,
            ListenToNotifications = newState, // Use the returned state
            NotificationToggled = true
        }, "API/toggle-notification-response");
    } else {
            Console.WriteLine("Notification toggle failed... sending response to API");
            _messageClient.Send(new ToggleNotificationMsg() {
                UserId = userId,
                FollowerId = followerId,
                ListenToNotifications = true, // Default value
                NotificationToggled = false
            }, "API/toggle-notification-response");
        }
    }

    public void FetchNotifListeners(int userId) {
    var notifListeners = _followersRepository.GetNotifListeners(userId);
    Console.WriteLine($"Fetching notification listeners for user with ID: {userId}");

    // Send the response to the Notification service
    _messageClient.Send(new FetchNotifListenersMsg() {
        UserId = userId,
        NotifListeners = notifListeners
    }, "FollowingService/fetch-notif-listeners-response");
}
}
