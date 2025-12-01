using UserApi.Model;

namespace UserApi.Messaging.Events;

public class FinancialAccountCreatedEvent : IOutboxEvent
{
    public string Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
}