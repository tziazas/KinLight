namespace KinLight.Modules.Shared;

/// <summary>The settings a family member edits on the portal's Settings page (ARCHITECTURE.md §13).</summary>
/// <param name="Name">The display's name, e.g. "Kitchen".</param>
/// <param name="ScreenShape">The screen's aspect ratio.</param>
/// <param name="Culture">The display language.</param>
/// <param name="FallbackCulture">The backup language, or null.</param>
/// <param name="TimeZoneId">IANA time zone id.</param>
/// <param name="NightMode">The night hours.</param>
public sealed record DisplaySettings(
    string Name,
    ScreenShape ScreenShape,
    string Culture,
    string? FallbackCulture,
    string TimeZoneId,
    NightMode NightMode);
