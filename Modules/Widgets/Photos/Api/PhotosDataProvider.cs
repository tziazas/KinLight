using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Photos.Client;

namespace KinLight.Modules.Widgets.Photos.Api;

/// <summary>Builds a placement's slides from the household library, filtered by the placement's album setting.</summary>
public sealed class PhotosDataProvider : IWidgetDataProvider
{
    private readonly IPhotoLibraryStore _store;

    /// <summary>Creates the provider.</summary>
    public PhotosDataProvider(IPhotoLibraryStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <inheritdoc />
    public string TypeKey => PhotosWidgetDefinition.Key;

    /// <inheritdoc />
    public TimeSpan RefreshInterval => TimeSpan.FromMinutes(10);

    /// <inheritdoc />
    public async Task<object> FetchAsync(WidgetDataRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var settings = (PhotosSettings)WidgetSettingsJson.Read(request.SettingsJson, typeof(PhotosSettings));
        var album = string.IsNullOrWhiteSpace(settings.Album) ? null : settings.Album;
        var photos = await _store.ListAsync(request.HouseholdId, album, cancellationToken).ConfigureAwait(false);

        // The display loads images from the photo endpoint with its device token; the URL carries no secret.
        var slides = photos
            .OrderBy(p => p.UploadedUtc)
            .Select(p => new PhotoSlide(p.Id, PhotosEndpointRouteBuilderExtensions.ImagePath(p.Id), p.Caption.IsEmpty ? null : p.Caption))
            .ToList();

        return new PhotosData(slides);
    }
}
