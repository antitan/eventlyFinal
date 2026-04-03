namespace Evently.Common.Application.EventBus;

/// <summary>
/// Contrat de publication d'événements d'intégration entre modules.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publie un événement sur le bus de messages.
    /// </summary>
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}
