namespace UserApi.Messaging;

public interface IMessageHandler<in T>
{
    Task HandleAsync(T evt, CancellationToken cancellationToken);
}