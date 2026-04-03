using Evently.Common.Domain;
using Microsoft.AspNetCore.Identity;

namespace Evently.Modules.Users.Domain.Users;

/// <summary>
/// Représente l'agrégat racine utilisateur dans le domaine Users.
/// </summary>
public sealed class User : IdentityUser<Guid>, IDomainEventEntity
{
    private readonly List<Role> _roles = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    private User()
    {
    }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string IdentityId { get; private set; } = string.Empty;

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    /// <summary>
    /// Fabrique un nouvel utilisateur membre et publie l'événement de création.
    /// </summary>
    public static User Create(string email, string firstName, string lastName, string identityId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            NormalizedEmail = email.ToUpperInvariant(),
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId,
        };

        user._roles.Add(Role.Member);

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    /// <summary>
    /// Met à jour le profil utilisateur et émet un événement uniquement si une valeur change.
    /// </summary>
    public void Update(string firstName, string lastName)
    {
        if (FirstName == firstName && LastName == lastName)
        {
            return;
        }

        FirstName = firstName;
        LastName = lastName;

        Raise(new UserProfileUpdatedDomainEvent(Id, FirstName, LastName));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
