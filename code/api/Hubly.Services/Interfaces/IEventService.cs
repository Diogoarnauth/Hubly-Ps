namespace Hubly.api.Services.Interfaces
{
    public interface IEventService
    {
        Task SendToTopic(string topic, string eventName, object data);
        Task SendToUser(string userId, string eventName, object data);
    }
}