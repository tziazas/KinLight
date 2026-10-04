namespace KinLight.Modules.Widgets.Clock.Client;

/// <summary>
/// Marker type for the clock widget's <c>.resx</c> strings (ARCHITECTURE.md §15).
/// Every resource is a whole sentence for one case; sentences are never assembled from fragments.
/// The constants name the resource keys.
/// </summary>
public sealed class ClockStrings
{
    /// <summary>The widget's name in the portal.</summary>
    public const string WidgetName = nameof(WidgetName);

    /// <summary>"It is {0} morning." where {0} is the weekday name.</summary>
    public const string ItIsMorning = nameof(ItIsMorning);

    /// <summary>"It is {0} afternoon."</summary>
    public const string ItIsAfternoon = nameof(ItIsAfternoon);

    /// <summary>"It is {0} evening."</summary>
    public const string ItIsEvening = nameof(ItIsEvening);

    /// <summary>"It is night-time." The night sentence omits the weekday on purpose.</summary>
    public const string ItIsNight = nameof(ItIsNight);

    /// <summary>Settings label: show the time.</summary>
    public const string Settings_ShowTime = nameof(Settings_ShowTime);

    /// <summary>Settings label: show the date.</summary>
    public const string Settings_ShowDate = nameof(Settings_ShowDate);

    private ClockStrings()
    {
    }
}
