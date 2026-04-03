namespace Evently.Common.Domain;

/// <summary>
/// Contrat pour les entités qui collectent des événements de domaine.
/// </summary>
public interface IDomainEventEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
