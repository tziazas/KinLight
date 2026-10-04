namespace KinLight.Modules.Shared;

/// <summary>
/// The hours between which the display dims and says it is night-time (ARCHITECTURE.md §3).
/// Setting <see cref="StartsAt"/> and <see cref="EndsAt"/> to the same time turns night mode off.
/// </summary>
/// <param name="StartsAt">Local time night begins, e.g. 21:00.</param>
/// <param name="EndsAt">Local time night ends, e.g. 07:00. May be earlier than <paramref name="StartsAt"/> to span midnight.</param>
public readonly record struct NightMode(TimeOnly StartsAt, TimeOnly EndsAt)
{
    /// <summary>Night mode turned off.</summary>
    public static NightMode Off { get; } = new(TimeOnly.MinValue, TimeOnly.MinValue);

    /// <summary>A sensible default: 21:00 to 07:00.</summary>
    public static NightMode Default { get; } = new(new TimeOnly(21, 0), new TimeOnly(7, 0));

    /// <summary>True unless start and end are the same time.</summary>
    public bool IsEnabled => StartsAt != EndsAt;

    /// <summary>Whether <paramref name="localTime"/> falls within the night hours.</summary>
    public bool IsNight(TimeOnly localTime)
    {
        if (!IsEnabled)
        {
            return false;
        }

        return StartsAt < EndsAt
            ? localTime >= StartsAt && localTime < EndsAt
            : localTime >= StartsAt || localTime < EndsAt;
    }
}
