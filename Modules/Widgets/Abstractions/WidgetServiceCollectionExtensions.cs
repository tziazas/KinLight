using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Explicit widget registration (ARCHITECTURE.md §14). Trim-safe in WebAssembly, and adding or removing a widget is one line.
/// </summary>
public static class WidgetServiceCollectionExtensions
{
    /// <summary>Registers a widget definition.</summary>
    public static IServiceCollection AddWidget<TDefinition>(this IServiceCollection services)
        where TDefinition : class, IWidgetDefinition
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IWidgetDefinition, TDefinition>());
        services.TryAddSingleton<IWidgetCatalog, WidgetCatalog>();
        return services;
    }

    /// <summary>Registers a widget data provider (server only).</summary>
    public static IServiceCollection AddWidgetDataProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IWidgetDataProvider
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IWidgetDataProvider, TProvider>());
        return services;
    }
}
