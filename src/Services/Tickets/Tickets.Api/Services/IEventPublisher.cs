namespace Tickets.Api.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : class;
}