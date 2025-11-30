using FinancialService.Model;

namespace FinancialService.Messaging.Events;

public class FinancialAccountCreatedEvent : IOutboxEvent
{
    public string Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
}