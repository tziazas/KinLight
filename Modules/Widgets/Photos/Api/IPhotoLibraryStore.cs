using KinLight.Modules.Widgets.Photos.Client;

namespace KinLight.Modules.Widgets.Photos.Api;

/// <summary>
/// Household photo storage, implemented by the host (ARCHITECTURE.md §17: files go through IFileStore; local disk in the
/// public repository, Azure Blob in the private one). Every call is scoped to a household.
/// </summary>
public interface IPhotoLibraryStore
{
    /// <summary>Lists a household's photos, newest first, optionally only those in <paramref name="album"/>.</summary>
    Task<IReadOnlyList<PhotoItem>> ListAsync(Guid householdId, string? album, CancellationToken cancellationToken);

    /// <summary>Opens a photo's bytes, or null if it does not exist in this household.</summary>
    Task<(Stream Content, string ContentType)?> OpenAsync(Guid householdId, Guid photoId, CancellationToken cancellationToken);
}
