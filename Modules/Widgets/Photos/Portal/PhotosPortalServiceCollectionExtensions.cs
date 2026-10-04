using KinLight.Modules.Portal.Client.Settings;
using KinLight.Modules.Widgets.Photos.Client;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Photos.Portal;

/// <summary>Registers the photos widget's portal pieces.</summary>
public static class PhotosPortalServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Photos navigation entry and the album picker. The host must register an <see cref="IPhotoLibraryApi"/>
    /// and add this assembly to the router.
    /// </summary>
    public static IServiceCollection AddKinLightPhotosPortal(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddPortalNavItem<PhotosNavItem>()
            .AddWidgetSettingsEditor<PhotosSettings>(nameof(PhotosSettings.Album), typeof(AlbumPicker));
    }
}
