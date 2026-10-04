namespace KinLight.Modules.Widgets.Calendar.Client;

/// <summary>Marker type for the calendar widget's <c>.resx</c> strings. Whole sentences per case (ARCHITECTURE.md §15).</summary>
public sealed class CalendarStrings
{
    /// <summary>The widget's name in the portal.</summary>
    public const string WidgetName = nameof(WidgetName);

    /// <summary>"Today" heading.</summary>
    public const string Today = nameof(Today);

    /// <summary>"Nothing more is planned for today."</summary>
    public const string NothingMoreToday = nameof(NothingMoreToday);

    /// <summary>"All day" label for events without a time.</summary>
    public const string AllDay = nameof(AllDay);

    /// <summary>Settings label: events to show.</summary>
    public const string Settings_MaxEvents = nameof(Settings_MaxEvents);

    /// <summary>Settings label: show all-day events.</summary>
    public const string Settings_ShowAllDayEvents = nameof(Settings_ShowAllDayEvents);

    private CalendarStrings()
    {
    }
}
