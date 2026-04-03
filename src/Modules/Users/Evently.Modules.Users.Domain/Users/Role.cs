namespace Evently.Modules.Users.Domain.Users;

/// <summary>
/// Valeur de rôle attribuable à un utilisateur (Member, Administrator).
/// </summary>
public sealed class Role
{
    public static readonly Role Administrator = new("Administrator");
    public static readonly Role Member = new("Member");

    private Role(string name)
    {
        Name = name;
    }

    private Role()
    {
    }

    public string Name { get; private set; }
}
