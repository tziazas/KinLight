using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Clock.Client;

/// <summary>Registers the clock widget.</summary>
public static class ClockWidgetServiceCollectionExtensions
{
    /// <summary>Adds the clock and day widget.</summary>
    public static IServiceCollection AddKinLightClockWidget(this IServiceCollection services)
        => services.AddWidget<ClockWidgetDefinition>();
}
