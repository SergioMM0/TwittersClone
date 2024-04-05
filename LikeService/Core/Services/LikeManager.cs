using EasyNetQ;
using LikeService.Application.Clients;
using LikeService.Core.Entities;
using LikeService.Infrastructure.Repositories;
using RabbitMQMessages.Like;
using RabbitMQMessages.Post;
using RabbitMQMessages.User;

namespace LikeService.Core.Services;

public class LikeManager {
    private readonly LikeRepository _postRepository;
    private readonly MessageClient _messageClient;

    public LikeManager(LikeRepository postRepository) {
        _postRepository = postRepository;
        // Create a new message client for EF context issue
        _messageClient = new MessageClient(
            RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest"));
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
        
        var result = _postRepository.Add(like);

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
        }
    }
}
