using KinLight.Modules.Widgets.Calendar.Api;
using KinLight.Modules.Widgets.Medication.Api;
using KinLight.Modules.Widgets.Photos.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KinLight.Modules.Display.Api;

/// <summary>
/// Registers the services the display API endpoints need.
/// </summary>
public static class DisplayApiServiceCollectionExtensions
{
    /// <summary>
    /// Adds the KinLight display API services to <paramref name="services"/>.
    /// </summary>
    public static IServiceCollection AddKinLightDisplayApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton(TimeProvider.System);

        // Server side of every widget that fetches outside data. One line per widget.
        services.AddKinLightCalendarWidgetProvider();
        services.AddKinLightPhotosWidgetProvider();
        services.AddKinLightMedicationWidgetProvider();

        // Display API services are registered here as endpoints are added.
        return services;
    }
}
