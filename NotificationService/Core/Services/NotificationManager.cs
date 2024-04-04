using System.Runtime.CompilerServices;
using EasyNetQ;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotificationService.Application.Clients;
using RabbitMQMessages.Follow;
using RabbitMQMessages.Notification;

namespace NotificationService.Core.Services;
public class NotificationManager
{
    private readonly MessageClient _messageClient;

    public NotificationManager(MessageClient messageClient)
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
                    // Assuming Send is synchronous and there's no async version
                    _messageClient.Send(new ReceiveNotificationMsg
                    {
                        UserId = listenerId,
                        Message = $"User {userId} posted a new post with id {postId}!",
                        NotificationSent = true
                    }, "NotificationService/send-notification-response");
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
}
