using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace KinLight.Modules.Portal.Api;

/// <summary>
/// Maps the portal API endpoints into a host application.
/// </summary>
public static class PortalApiEndpointRouteBuilderExtensions
{
    /// <summary>The default route prefix for the portal API.</summary>
    public const string DefaultPrefix = "/api/portal";

    /// <summary>
    /// Maps the KinLight portal API under <paramref name="prefix"/>.
    /// Returns the route group so the host can apply further conventions (authorization, rate limiting).
    /// </summary>
    public static RouteGroupBuilder MapKinLightPortalApi(this IEndpointRouteBuilder endpoints, string prefix = DefaultPrefix)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        var group = endpoints.MapGroup(prefix).WithTags("Portal");

        // Portal endpoints (displays, layout, members, devices, content) are mapped onto this group as they are added.
        // Every endpoint here must enforce household membership and role (ARCHITECTURE.md §9, §13).
        return group;
    }
}
