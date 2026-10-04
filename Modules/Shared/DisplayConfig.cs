namespace KinLight.Modules.Shared;

/// <summary>
/// Everything a display needs to render (ARCHITECTURE.md §10, §11). Fetched from the API and cached
/// in the browser as the last good copy.
/// </summary>
/// <param name="DisplayId">The display id.</param>
/// <param name="Name">The display's name, e.g. "Kitchen".</param>
/// <param name="Culture">The display language (culture name), independent of the device and the portal.</param>
/// <param name="FallbackCulture">The backup language used when text is missing in <paramref name="Culture"/>.</param>
/// <param name="TimeZoneId">IANA time zone id, e.g. "Europe/Athens".</param>
/// <param name="NightMode">The night hours.</param>
/// <param name="ScreenShape">The physical screen's aspect ratio.</param>
/// <param name="Version">Concurrency token, bumped on every save.</param>
/// <param name="Widgets">The placed widgets.</param>
public sealed record DisplayConfig(
    Guid DisplayId,
    string Name,
    string Culture,
    string? FallbackCulture,
    string TimeZoneId,
    NightMode NightMode,
    ScreenShape ScreenShape,
    long Version,
    IReadOnlyList<WidgetPlacement> Widgets);
