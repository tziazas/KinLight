namespace KinLight.Modules.Widgets.Clock.Client;

/// <summary>The part of the day the clock speaks about.</summary>
public enum DayPart
{
    /// <summary>Night, per the display's night hours.</summary>
    Night,

    /// <summary>Before noon.</summary>
    Morning,

    /// <summary>Noon to 17:00.</summary>
    Afternoon,

    /// <summary>17:00 onwards.</summary>
    Evening,
}

/// <summary>Decides the day part for a local time.</summary>
public static class DayParts
{
    /// <summary>Returns the day part, treating configured night hours as <see cref="DayPart.Night"/>.</summary>
    public static DayPart For(TimeOnly localTime, bool isNight)
    {
        if (isNight)
        {
            return DayPart.Night;
        }

        return localTime.Hour switch
        {
            < 12 => DayPart.Morning,
            < 17 => DayPart.Afternoon,
            _ => DayPart.Evening,
        };
    }
}
