using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Photos.Api;

/// <summary>Registers the photos widget's server side.</summary>
public static class PhotosApiServiceCollectionExtensions
{
    /// <summary>Adds the photos data provider. The host must also register an <see cref="IPhotoLibraryStore"/>.</summary>
    public static IServiceCollection AddKinLightPhotosWidgetProvider(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddWidgetDataProvider<PhotosDataProvider>();
    }
}
