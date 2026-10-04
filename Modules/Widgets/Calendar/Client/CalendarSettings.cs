using System.ComponentModel.DataAnnotations;

namespace KinLight.Modules.Widgets.Calendar.Client;

/// <summary>
/// Settings for the calendar widget. These travel to the display, so they hold nothing secret:
/// the calendar's feed address is stored encrypted on the server.
/// </summary>
public sealed class CalendarSettings
{
    /// <summary>How many upcoming events to show at most.</summary>
    [Display(Name = nameof(CalendarStrings.Settings_MaxEvents))]
    [Range(1, 10)]
    public int MaxEvents { get; set; } = 4;

    /// <summary>Whether to show all-day events.</summary>
    [Display(Name = nameof(CalendarStrings.Settings_ShowAllDayEvents))]
    public bool ShowAllDayEvents { get; set; } = true;
}
