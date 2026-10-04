using System.Globalization;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Shared;
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

    /// <inheritdoc />
    /// <remarks>Caregivers have no access to the photo library (ARCHITECTURE.md §13).</remarks>
    public bool IsVisibleTo(MemberRole role) => role != MemberRole.Caregiver;
}
