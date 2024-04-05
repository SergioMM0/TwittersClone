using EasyNetQ;
using LikeService.Application.Clients;
using LikeService.Application.Interfaces.Clients;
using LikeService.Application.Interfaces.Repositories;
using LikeService.Core.Entities;
using LikeService.Infrastructure.Repositories;
using RabbitMQMessages.Like;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;
using RabbitMQMessages.Notification;

namespace LikeService.Core.Services;

public class LikeManager {
    private readonly ILikeRepository _likeRepository;
    private readonly IMessageClient _messageClient;

    public LikeManager(ILikeRepository likeRepository, IMessageClient messageClient) {
        _likeRepository = likeRepository;
        _messageClient = messageClient;
    }
    public async Task AddLike(int postId, int userId) {
        var receiverTopicUser = "LikeService/checkUserExists-response";
        var taskUser = _messageClient.ListenAsync<ResponseUserExistsMsg>(receiverTopicUser);
        
        Console.WriteLine("Checking that user with id: " + userId + " exists...");
        
        _messageClient.Send(new UserExistsReqMsg()
        {
            Id = userId,
            ReceiverTopic = receiverTopicUser
        }, "UserService/checkUserExists");
        
        var userExists = await taskUser;
        
        if (!userExists.Success) {
            Console.WriteLine("User was not found... sending response to API");
            _messageClient.Send(new LikeAddedMsg()
            {
                Success = false,
                Reason = "User with given id not found"
            }, "API/like-added");
            return;
        }
        
        var receiverTopicPost = "LikeService/checkUserExists-response";
        var taskPost = _messageClient.ListenAsync<ResponsePostExists>(receiverTopicPost);
        
        Console.WriteLine("Checking that post with id: " + postId + " exists...");
        
        _messageClient.Send(new PostExistsReqMsg()
        {
            Id = postId,
            ReceiverTopic = receiverTopicPost
        }, "PostService/checkPostExists");
        
        var postExists = await taskPost;
        
        if (!postExists.Success) {
            Console.WriteLine("Post was not found... sending response to API");
            _messageClient.Send(new LikeAddedMsg()
            {
                Success = false,
                Reason = "Post with given id not found"
            }, "API/like-added");
            return;
        }

        var like = new Like() {
            PostId = postId,
            UserId = userId
        };
        
        var result = _likeRepository.Add(like);

        if (result is null)
        {
            Console.WriteLine("Like creation failed... sending response to API");
            _messageClient.Send(new LikeAddedMsg()
            {
                Success = false,
                Reason = "Couldn't create like in database"
            }, "API/like-added");
        }
        else
        {
            Console.WriteLine("Like created successfully... sending response to API");
            _messageClient.Send(new LikeAddedMsg()
            {
                Success = true
            }, "API/like-added");

            // Send a like notification to the post author
            Console.WriteLine("Sending like notification to author of post with id: " + postId);
            _messageClient.Send(new SendLikeNotifReqMsg() {
                UserId = userId,
                PostId = postId
            }, "NotificationService/send-like-notification-request");
        }
    }
}
