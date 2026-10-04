using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Portal.Api;

/// <summary>
/// Registers the services the portal API endpoints need.
/// </summary>
public static class PortalApiServiceCollectionExtensions
{
    /// <summary>
    /// Adds the KinLight portal API services to <paramref name="services"/>.
    /// </summary>
    public static IServiceCollection AddKinLightPortalApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Portal API services are registered here as endpoints are added.
        return services;
    }
}
