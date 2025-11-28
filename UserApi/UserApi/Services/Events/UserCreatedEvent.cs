namespace UserApi.Services.Events;

public record UserCreatedEvent : OutboxEvent
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}