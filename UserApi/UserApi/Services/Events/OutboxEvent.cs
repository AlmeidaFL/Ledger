namespace UserApi.Services.Events;

public interface OutboxEvent
{
    public string Id { get; set; }
}