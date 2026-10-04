using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>Registers the photos widget's browser side.</summary>
public static class PhotosWidgetServiceCollectionExtensions
{
    /// <summary>Adds the photos widget.</summary>
    public static IServiceCollection AddKinLightPhotosWidget(this IServiceCollection services)
        => services.AddWidget<PhotosWidgetDefinition>();
}
