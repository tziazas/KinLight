using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Portal.Client;

/// <summary>A section another assembly contributes to the Today page, rendered in <see cref="Order"/>.</summary>
/// <param name="Order">Sort order.</param>
/// <param name="Component">The component to render. It receives no parameters.</param>
public sealed record TodaySectionRegistration(int Order, Type Component);

/// <summary>A component another assembly places in the portal's app bar (e.g. a development member switcher).</summary>
/// <param name="Component">The component to render.</param>
public sealed record AppBarItemRegistration(Type Component);

/// <summary>Registration helpers for the portal's extension points (ARCHITECTURE.md §13).</summary>
public static class PortalExtensionServiceCollectionExtensions
{
    /// <summary>Contributes a Today page section.</summary>
    public static IServiceCollection AddPortalTodaySection<TComponent>(this IServiceCollection services, int order)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new TodaySectionRegistration(order, typeof(TComponent)));
        return services;
    }

    /// <summary>Places a component in the app bar.</summary>
    public static IServiceCollection AddPortalAppBarItem<TComponent>(this IServiceCollection services)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new AppBarItemRegistration(typeof(TComponent)));
        return services;
    }
}
