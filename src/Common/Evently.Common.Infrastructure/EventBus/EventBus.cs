using Evently.Common.Application.EventBus;
using MassTransit;

namespace Evently.Common.Infrastructure.EventBus;

/// <summary>
/// Adaptateur concret vers MassTransit pour publier des IntegrationEvents.
/// </summary>
internal sealed class EventBus(IBus bus) : IEventBus
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        // Publication asynchrone sur le bus configuré (in-memory / broker réel).
        await bus.Publish(integrationEvent, cancellationToken);
    }
}
