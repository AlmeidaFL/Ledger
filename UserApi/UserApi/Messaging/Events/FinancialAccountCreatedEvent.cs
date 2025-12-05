using ServiceCommons;
using UserApi.Model;

namespace UserApi.Messaging.Events;

public class FinancialAccountCreatedEvent : IOutboxEvent
{
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public Guid UserId { get; set; }
}