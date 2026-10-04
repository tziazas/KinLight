using KinLight.Modules.Shared;

namespace KinLight.Modules.Portal.Client;

/// <summary>
/// The signed-in household member. Backed by ASP.NET Core Identity in production; development hosts provide a fake
/// with a member switcher. Confirmation logs record <see cref="Id"/> and <see cref="Name"/>, so this is never anonymous.
/// </summary>
public interface ICurrentMember
{
    /// <summary>The member id.</summary>
    Guid Id { get; }

    /// <summary>The member's name as shown to the household.</summary>
    string Name { get; }

    /// <summary>The member's role.</summary>
    MemberRole Role { get; }

    /// <summary>Raised when the member changes (sign-in, sign-out, or the development switcher).</summary>
    event Action? Changed;
}
