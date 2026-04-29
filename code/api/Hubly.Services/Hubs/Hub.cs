using Microsoft.AspNetCore.SignalR;

namespace  Hubly.api.Services.Hubs;

public class HublyHub : Hub 
{
    public async Task JoinTopic(string topicName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topicName);
        Console.WriteLine($"Client {Context.ConnectionId} joined topic: {topicName}");
    }

    public async Task LeaveTopic(string topicName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topicName);
    }
}