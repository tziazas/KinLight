using KinLight.Modules.Shared;

namespace KinLight.Modules.Portal.Client;

/// <summary>
/// What the portal needs from the server. Implemented over HTTP in production and by an in-browser fake in
/// development hosts. Every call is scoped to the signed-in member's household and role (ARCHITECTURE.md §9, §13).
/// </summary>
public interface IPortalApi
{
    /// <summary>Lists the household's displays.</summary>
    Task<IReadOnlyList<DisplaySummary>> ListDisplaysAsync(CancellationToken cancellationToken);

    /// <summary>Loads one display, or null if it does not exist in this household.</summary>
    Task<DisplayConfig?> GetDisplayAsync(Guid displayId, CancellationToken cancellationToken);

    /// <summary>Saves the whole layout. Returns the new version. Throws <see cref="VersionConflictException"/> on a stale version.</summary>
    Task<long> SaveLayoutAsync(SaveDisplayLayoutRequest request, CancellationToken cancellationToken);

    /// <summary>Updates the display's settings. Returns the new version. Throws <see cref="VersionConflictException"/> on a stale version.</summary>
    Task<long> UpdateSettingsAsync(UpdateDisplaySettingsRequest request, CancellationToken cancellationToken);
}
