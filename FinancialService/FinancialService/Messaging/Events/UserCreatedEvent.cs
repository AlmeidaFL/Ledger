namespace FinancialService.Messaging.Events;

public class UserCreatedEvent
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}