using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>A photo in the household's library, as the portal sees it.</summary>
/// <param name="Id">The photo id.</param>
/// <param name="Caption">Who this is to the viewer, per language.</param>
/// <param name="Albums">Album names the photo belongs to. Free-form; the set of albums is whatever photos use.</param>
/// <param name="UploadedUtc">When it was uploaded.</param>
public sealed record PhotoItem(Guid Id, LocalizedText Caption, IReadOnlyList<string> Albums, DateTimeOffset UploadedUtc);

/// <summary>A photo being uploaded. Already downscaled by the portal; never a raw camera original.</summary>
/// <param name="Content">Image bytes.</param>
/// <param name="ContentType">MIME type, e.g. image/jpeg.</param>
/// <param name="FileName">Original file name, for the library only.</param>
/// <param name="Caption">Initial caption, may be empty.</param>
/// <param name="Albums">Initial albums.</param>
public sealed record PhotoUpload(byte[] Content, string ContentType, string FileName, LocalizedText Caption, IReadOnlyList<string> Albums);

/// <summary>
/// The portal's view of the household photo library (ARCHITECTURE.md §14, §17). Implemented over HTTP in production
/// (files served only through authorized endpoints) and by an in-browser fake in development hosts.
/// Every call is scoped to the signed-in member's household.
/// </summary>
public interface IPhotoLibraryApi
{
    /// <summary>Lists the household's photos, newest first.</summary>
    Task<IReadOnlyList<PhotoItem>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Stores a photo.</summary>
    Task<PhotoItem> UploadAsync(PhotoUpload upload, CancellationToken cancellationToken);

    /// <summary>Updates a photo's caption and albums.</summary>
    Task<PhotoItem> UpdateAsync(Guid id, LocalizedText caption, IReadOnlyList<string> albums, CancellationToken cancellationToken);

    /// <summary>Deletes a photo everywhere, including from every display that shows it.</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>A URL the portal can show the image from, or null if the photo is gone.</summary>
    Task<string?> GetImageUrlAsync(Guid id, CancellationToken cancellationToken);
}
