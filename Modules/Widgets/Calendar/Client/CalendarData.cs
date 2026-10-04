using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Calendar.Client;

/// <summary>
/// Today's events as fetched by the server. Times are local to the display.
/// </summary>
/// <param name="Date">The local date the events belong to.</param>
/// <param name="Events">The events, in start order.</param>
public sealed record CalendarData(DateOnly Date, IReadOnlyList<CalendarEventItem> Events)
{
    /// <summary>No events.</summary>
    public static CalendarData Empty(DateOnly date) => new(date, []);
}

/// <summary>One event on the display's calendar.</summary>
/// <param name="Title">The event title.</param>
/// <param name="Start">Local start time; null for all-day events.</param>
/// <param name="End">Local end time; null for all-day events.</param>
/// <param name="IsAllDay">Whether the event lasts all day.</param>
public sealed record CalendarEventItem(LocalizedText Title, TimeOnly? Start, TimeOnly? End, bool IsAllDay);
