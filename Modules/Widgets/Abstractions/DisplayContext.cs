using System.Globalization;
using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// What every widget needs to know about the display it is rendered on (ARCHITECTURE.md §14).
/// Provided to widgets as a cascading parameter.
/// </summary>
public sealed class DisplayContext
{
    /// <summary>Creates a display context.</summary>
    public DisplayContext(CultureInfo culture, CultureInfo? fallbackCulture, TimeZoneInfo timeZone, TimeProvider clock, NightMode nightMode)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        FallbackCulture = fallbackCulture;
        TimeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        NightMode = nightMode;
    }

    /// <summary>The display language.</summary>
    public CultureInfo Culture { get; }

    /// <summary>The backup language for text missing in <see cref="Culture"/>.</summary>
    public CultureInfo? FallbackCulture { get; }

    /// <summary>The display's time zone.</summary>
    public TimeZoneInfo TimeZone { get; }

    /// <summary>The clock. Injected so tests can fix the time.</summary>
    public TimeProvider Clock { get; }

    /// <summary>The night hours.</summary>
    public NightMode NightMode { get; }

    /// <summary>The current local date and time on the display.</summary>
    public DateTime LocalNow => TimeZoneInfo.ConvertTime(Clock.GetUtcNow(), TimeZone).DateTime;

    /// <summary>Whether it is currently night-time on the display.</summary>
    public bool IsNight => NightMode.IsNight(TimeOnly.FromDateTime(LocalNow));

    /// <summary>Resolves caregiver-entered text for this display.</summary>
    /// <param name="text">The text.</param>
    /// <param name="allowUnreviewed">False for safety-relevant text, which never shows an unreviewed machine translation.</param>
    public string? Resolve(LocalizedText? text, bool allowUnreviewed = true) => text?.Resolve(Culture, FallbackCulture, allowUnreviewed);
}
