using System.ComponentModel.DataAnnotations;

namespace KinLight.Modules.Widgets.Clock.Client;

/// <summary>Settings for the clock widget. The portal generates its form from these annotations.</summary>
public sealed class ClockSettings
{
    /// <summary>Whether to show the time of day under the day sentence.</summary>
    [Display(Name = nameof(ClockStrings.Settings_ShowTime))]
    public bool ShowTime { get; set; } = true;

    /// <summary>Whether to show the long date under the time.</summary>
    [Display(Name = nameof(ClockStrings.Settings_ShowDate))]
    public bool ShowDate { get; set; } = true;
}
