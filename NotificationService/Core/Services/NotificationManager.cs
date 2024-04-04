using System.Runtime.CompilerServices;
using EasyNetQ;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotificationService.Application.Clients;
using RabbitMQMessages.Follow;
using RabbitMQMessages.Notification;

namespace NotificationService.Core.Services; 
public class NotificationManager {
    private readonly MessageClient _messageClient;

    public NotificationManager(MessageClient messageClient) {
        _messageClient = messageClient;
    }

    public void SendNotificationMsg(int userId, int postId) {
        // Fetch all the listening followers of the user
        _messageClient.Send(new FetchNotifListenersReqMsg() {
            UserId = userId
        }, "FollowingService/fetch-notif-listeners-request");
        _messageClient.Listen<FetchNotifListenersMsg>(HandleFetchNotifListenersResponse, "FollowingService/fetch-notif-listeners-response");

        void HandleFetchNotifListenersResponse(FetchNotifListenersMsg msg) {
            // Send the notification to all the listeners
            foreach (var listener in msg.NotifListeners) {
                _messageClient.Send(new SendNotificationMsg() {
                    UserId = userId,
                    PostId = postId,
                    FollowerId = msg.NotifListeners,
                    Message = userId +" posted a new post with id "+ postId +"!"
                }, "NotificationService/send-notification-request");
            }
        }
    }
}
