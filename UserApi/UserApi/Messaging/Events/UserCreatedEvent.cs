using ServiceCommons;
using UserApi.Model;

namespace UserApi.Messaging.Events;

public record UserCreatedEvent : IOutboxEvent
{
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public string Email { get; set; }
    public string FullName { get; set; }
}