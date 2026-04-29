using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace Hubly.api.Services
{
    public class EventService : IEventService
    {
        private readonly IHubContext<HublyHub> _hubContext;

        public EventService(
            IHubContext<HublyHub> hubContext
        )
        {
            _hubContext = hubContext;
        }

        public async Task SendToTopic(string topic, string eventName, object data)
        {
            await _hubContext.Clients.Group(topic).SendAsync(eventName, data);
        }

        public async Task SendToUser(string userId, string eventName, object data)
        {
            await _hubContext.Clients.User(userId).SendAsync(eventName, data);
        }

        
    }
}