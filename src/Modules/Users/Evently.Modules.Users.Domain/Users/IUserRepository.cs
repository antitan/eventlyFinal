namespace Evently.Modules.Users.Domain.Users;

/// <summary>
/// Contrat de persistance pour l'agrégat <see cref="User"/>.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(User user);
}
