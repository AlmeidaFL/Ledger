namespace UserApi.Services.Events;

public record UserCreatedEvent(Guid UserId, string Email, string FullName);