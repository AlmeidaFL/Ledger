using ServiceCommons;
using UserApi.Model;

namespace UserApi.Messaging.Events;

public class UserCreatedEvent : IOutboxEvent
{
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
}