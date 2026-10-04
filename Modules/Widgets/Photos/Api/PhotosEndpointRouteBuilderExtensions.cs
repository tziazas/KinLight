using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace KinLight.Modules.Widgets.Photos.Api;

/// <summary>Maps the photo image endpoint the display and portal load pictures from.</summary>
public static class PhotosEndpointRouteBuilderExtensions
{
    /// <summary>Relative path of a photo's image, under the display API prefix.</summary>
    public static string ImagePath(Guid photoId) => $"photos/{photoId}/image";

    /// <summary>
    /// Maps <c>GET photos/{id}/image</c> onto <paramref name="group"/>. The group's authorization (device token or
    /// portal cookie) decides who may call it, and the household comes from <paramref name="resolveHousehold"/>.
    /// </summary>
    public static RouteGroupBuilder MapKinLightPhotoImages(this RouteGroupBuilder group, Func<HttpContext, Guid?> resolveHousehold)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(resolveHousehold);

        group.MapGet("photos/{id:guid}/image", async (Guid id, HttpContext http, IPhotoLibraryStore store, CancellationToken ct) =>
        {
            var householdId = resolveHousehold(http);
            if (householdId is null)
            {
                return Results.Unauthorized();
            }

            var opened = await store.OpenAsync(householdId.Value, id, ct).ConfigureAwait(false);
            if (opened is null)
            {
                return Results.NotFound();
            }

            http.Response.Headers.CacheControl = "private, max-age=86400";
            return Results.Stream(opened.Value.Content, opened.Value.ContentType);
        }).WithName("KinLightPhotoImage");

        return group;
    }
}
