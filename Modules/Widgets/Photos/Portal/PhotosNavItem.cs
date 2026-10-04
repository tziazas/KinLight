using System.Globalization;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Widgets.Abstractions;
using MudBlazor;

namespace KinLight.Modules.Widgets.Photos.Portal;

/// <summary>The "Photos" entry in the portal navigation.</summary>
public sealed class PhotosNavItem : IPortalNavItem
{
    /// <inheritdoc />
    public string Href => "portal/photos";

    /// <inheritdoc />
    public string Icon => Icons.Material.Filled.PhotoLibrary;

    /// <inheritdoc />
    public int Order => 10;

    /// <inheritdoc />
    public string Label => WidgetStrings.For<PhotosPortalStrings>(CultureInfo.CurrentUICulture)[PhotosPortalStrings.Nav_Photos];
}
