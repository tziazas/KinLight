using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace KinLight.Modules.Display.Api;

/// <summary>
/// Maps the display API endpoints into a host application.
/// </summary>
public static class DisplayApiEndpointRouteBuilderExtensions
{
    /// <summary>The default route prefix for the display API.</summary>
    public const string DefaultPrefix = "/api/display";

    /// <summary>
    /// Maps the KinLight display API under <paramref name="prefix"/>.
    /// Returns the route group so the host can apply further conventions (authorization, rate limiting).
    /// </summary>
    public static RouteGroupBuilder MapKinLightDisplayApi(this IEndpointRouteBuilder endpoints, string prefix = DefaultPrefix)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        var group = endpoints.MapGroup(prefix).WithTags("Display");

        // Display endpoints (config, pairing, widget data) are mapped onto this group as they are added.
        return group;
    }
}
