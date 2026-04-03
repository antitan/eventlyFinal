using Microsoft.AspNetCore.Identity;

namespace Evently.Modules.Users.Domain.Users;

/// <summary>
/// Valeur de rôle attribuable à un utilisateur (Member, Administrator).
/// </summary>
public sealed class Role : IdentityRole<Guid>
{
    public static readonly Role Administrator = new(Guid.Parse("7e7e4db6-0880-4b6a-b5d3-2d50e183f960"), "Administrator");
    public static readonly Role Member = new(Guid.Parse("43f0ccca-e92a-4fa9-8d76-95e0746d33f6"), "Member");

    private Role(Guid id, string name)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
    }

    private Role()
    {
    }
}
