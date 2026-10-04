namespace KinLight.Modules.Portal.Client;

/// <summary>
/// A navigation entry contributed to the portal by another assembly (ARCHITECTURE.md §13, extension points), such as a
/// widget's library page. Registered in DI; the layout renders them after the built-in entries, by <see cref="Order"/>.
/// </summary>
public interface IPortalNavItem
{
    /// <summary>Relative href without a leading slash, e.g. "portal/photos".</summary>
    string Href { get; }

    /// <summary>A MudBlazor icon (SVG path string).</summary>
    string Icon { get; }

    /// <summary>Sort order among contributed items.</summary>
    int Order { get; }

    /// <summary>The label in the portal's current language.</summary>
    string Label { get; }
}
