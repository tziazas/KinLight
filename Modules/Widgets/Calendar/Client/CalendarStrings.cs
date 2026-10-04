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

    private CalendarStrings()
    {
    }
}
