using FollowersService.Application.Clients;

namespace FollowersService.Core.Services; 
public class FollowingService {
    private readonly MessageClient _messageClient;

    public FollowingService(MessageClient messageClient) {
        _messageClient = messageClient;
    }
}
