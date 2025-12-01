using UserApi.Model;

namespace UserApi.Messaging.Events;

public record UserCreatedEvent : IOutboxEvent
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}