using EasyNetQ;
using LikeService.Application.Clients;
using LikeService.Infrastructure.Repositories;

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
}
