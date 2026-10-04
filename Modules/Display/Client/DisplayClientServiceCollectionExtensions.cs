using KinLight.Modules.Widgets.BuiltIn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KinLight.Modules.Display.Client;

/// <summary>
/// Registers the services the display's Blazor WebAssembly components need.
/// </summary>
public static class DisplayClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds the KinLight display client services, including the built-in widgets, to <paramref name="services"/>.
    /// </summary>
    public static IServiceCollection AddKinLightDisplayClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Built-in text (.resx) resolves through IStringLocalizer (ARCHITECTURE.md §15).
        services.AddLocalization();
        services.AddKinLightBuiltInWidgets();
        services.TryAddSingleton(TimeProvider.System);

        // The host must register an IDisplayApi (HTTP + SignalR in production, a fake in development hosts).
        return services;
    }
}
