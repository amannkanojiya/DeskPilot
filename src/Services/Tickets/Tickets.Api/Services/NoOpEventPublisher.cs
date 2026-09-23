namespace Tickets.Api.Services;

// Temporary stand-in until RabbitMQ/MassTransit is wired in the next step.
// Controllers already call IEventPublisher, so swapping this for a real
// implementation later requires zero controller changes.
public class NoOpEventPublisher : IEventPublisher
{
    private readonly ILogger<NoOpEventPublisher> _logger;
    public NoOpEventPublisher(ILogger<NoOpEventPublisher> logger) => _logger = logger;

    public Task PublishAsync<T>(T @event) where T : class
    {
        _logger.LogInformation("Event published (no-op): {Event}", @event);
        return Task.CompletedTask;
    }
}