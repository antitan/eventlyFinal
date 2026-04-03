namespace Evently.Common.Domain;

/// <summary>
/// Classe de base des agrégats/entités qui publient des événements de domaine.
/// </summary>
public abstract class Entity : IDomainEventEntity
{
    // Buffer temporaire des événements produits durant une unité de travail.
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity()
    {
    }

    /// <summary>
    /// Expose une copie en lecture seule des événements actuellement collectés.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    /// <summary>
    /// Vide les événements après leur persistance dans l'outbox.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Ajoute un événement de domaine à publier en fin de transaction.
    /// </summary>
    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
