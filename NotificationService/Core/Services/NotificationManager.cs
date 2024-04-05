using System.Runtime.CompilerServices;
using EasyNetQ;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotificationService.Application.Clients;
using NotificationService.Application.Interfaces.Clients;
using RabbitMQMessages.Follow;
using RabbitMQMessages.Post;
using RabbitMQMessages.Notification;

namespace NotificationService.Core.Services;
public class NotificationManager
{
    private readonly IMessageClient _messageClient;

    public NotificationManager(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task<bool> SendNotificationMsg(int userId, int postId)
    {
        var tcs = new TaskCompletionSource<bool>();

        void HandleFetchNotifListenersResponse(FetchNotifListenersMsg msg)
        {
            try
            {
                foreach (var listenerId in msg.NotifListeners)
                {
                    Console.WriteLine("Notification: Hey "+listenerId+"! User with id: " + userId + " posted a new post with id: " + postId);
                }
                tcs.SetResult(true); // Notify the task completion
            }
            catch (Exception ex)
            {
                tcs.SetException(ex); // Pass any exceptions to the Task
            }
        }

        // Setup listening before sending the request to ensure it's ready to catch the response
        _messageClient.Listen<FetchNotifListenersMsg>(HandleFetchNotifListenersResponse, "NotificationService/fetch-notif-listeners-response");

        // Send request
        _messageClient.Send(new FetchNotifListenersReqMsg { UserId = userId }, "FollowingService/fetch-notif-listeners-request");

        return await tcs.Task; // Wait here until HandleFetchNotifListenersResponse signals completion
    }

    public async Task SendLikeNotifMsg(int userId, int postId)
    {   
        var receiverTopic = "Notification/getPostAuthorById-response";
        var task = _messageClient.ListenAsync<PostAuthorMsg>(receiverTopic);

        Console.WriteLine("Getting post author by post id: " + postId + "...");

        // Fetch the author of the post
        _messageClient.Send(new GetPostAuthorById { Id = postId }, "PostService/getPostAuthorById-request");

        var postAuthor = await task;

        if (!postAuthor.Success)
        {
            Console.WriteLine("Post author not found... sending response to API");
            _messageClient.Send(new 
            {
                Success = false,
                Reason = "Post author not found"
            }, "API/like-added");
            return;
        }
        else
        {
            Console.WriteLine("Post author found... sending like notification to author");

            // Send a notification to the post author that the user liked their post
            Console.WriteLine("Notification: Hey "+postAuthor.AuthorId+"! User with id: " + userId + " liked your post with id: " + postId);
        }
    }
}
