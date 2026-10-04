using KinLight.Modules.Widgets.Calendar.Client;
using KinLight.Modules.Widgets.Clock.Client;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.BuiltIn;

/// <summary>Registers every built-in widget. Called by both the display and the portal.</summary>
public static class BuiltInWidgetServiceCollectionExtensions
{
    /// <summary>Adds all built-in widgets, explicitly and in a fixed order.</summary>
    public static IServiceCollection AddKinLightBuiltInWidgets(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddKinLightClockWidget()
            .AddKinLightCalendarWidget();
    }
}
