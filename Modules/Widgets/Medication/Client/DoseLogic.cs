namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>Where a dose stands right now.</summary>
public enum DoseStatus
{
    /// <summary>Before its window.</summary>
    Upcoming = 0,

    /// <summary>Inside its window and not yet confirmed.</summary>
    Due = 1,

    /// <summary>Confirmed by a caregiver.</summary>
    Taken = 2,

    /// <summary>Window closed without confirmation. The display stays silent; the family is alerted.</summary>
    Missed = 3,
}

/// <summary>A scheduled dose on a particular day.</summary>
/// <param name="ScheduleId">The medication.</param>
/// <param name="Date">Local date.</param>
/// <param name="At">Local time due.</param>
/// <param name="WindowMinutes">Length of the window after <paramref name="At"/>.</param>
public sealed record DoseOccurrence(Guid ScheduleId, DateOnly Date, TimeOnly At, int WindowMinutes)
{
    /// <summary>Local start of the window.</summary>
    public DateTime WindowStart => Date.ToDateTime(At);

    /// <summary>Local end of the window.</summary>
    public DateTime WindowEnd => WindowStart.AddMinutes(Math.Max(1, WindowMinutes));
}

/// <summary>A dose as the portal and display see it for one day, with its status.</summary>
/// <param name="Schedule">The medication.</param>
/// <param name="At">Local time due (or given, for as-needed).</param>
/// <param name="WindowMinutes">Window length; 0 for as-needed.</param>
/// <param name="Status">Current status.</param>
/// <param name="Confirmation">The confirmation, when taken.</param>
/// <param name="IsAsNeeded">True for an as-needed dose that was logged.</param>
public sealed record DayDose(MedicationSchedule Schedule, TimeOnly At, int WindowMinutes, DoseStatus Status, DoseConfirmation? Confirmation, bool IsAsNeeded);

/// <summary>Pure scheduling and status rules. No clock, no I/O: everything is passed in, so it is fully testable.</summary>
public static class DoseLogic
{
    /// <summary>The scheduled doses of <paramref name="schedule"/> on <paramref name="date"/>, in time order.</summary>
    public static IEnumerable<DoseOccurrence> OccurrencesOn(MedicationSchedule schedule, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!AppliesOn(schedule, date))
        {
            return [];
        }

        return schedule.Doses.OrderBy(d => d.At).Select(d => new DoseOccurrence(schedule.Id, date, d.At, d.WindowMinutes));
    }

    /// <summary>Whether a schedule has doses on a date (as-needed never does).</summary>
    public static bool AppliesOn(MedicationSchedule schedule, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (!schedule.IsActive || schedule.Doses.Count == 0)
        {
            return false;
        }

        if (schedule.StartsOn is { } start && date < start)
        {
            return false;
        }

        if (schedule.EndsOn is { } end && date > end)
        {
            return false;
        }

        return schedule.Kind switch
        {
            ScheduleKind.Daily => true,
            ScheduleKind.Weekly => schedule.Weekdays.Contains(date.DayOfWeek),
            ScheduleKind.EveryNDays => schedule.EveryNDays > 0
                && schedule.AnchorDate is { } anchor
                && date >= anchor
                && (date.DayNumber - anchor.DayNumber) % schedule.EveryNDays == 0,
            ScheduleKind.SpecificDates => schedule.Dates.Contains(date),
            _ => false,
        };
    }

    /// <summary>Status of one occurrence given its confirmation (if any) and the local time now.</summary>
    public static DoseStatus StatusOf(DoseOccurrence occurrence, DoseConfirmation? confirmation, DateTime localNow)
    {
        ArgumentNullException.ThrowIfNull(occurrence);

        if (confirmation is not null)
        {
            return DoseStatus.Taken;
        }

        if (localNow < occurrence.WindowStart)
        {
            return DoseStatus.Upcoming;
        }

        return localNow < occurrence.WindowEnd ? DoseStatus.Due : DoseStatus.Missed;
    }

    /// <summary>
    /// Everything that happened or should happen on <paramref name="date"/>: scheduled doses with their status, plus
    /// as-needed doses that were logged. Ordered by time.
    /// </summary>
    public static IReadOnlyList<DayDose> BuildDay(IEnumerable<MedicationSchedule> schedules, IEnumerable<DoseConfirmation> confirmations, DateOnly date, DateTime localNow)
    {
        ArgumentNullException.ThrowIfNull(schedules);
        ArgumentNullException.ThrowIfNull(confirmations);

        var scheduleList = schedules.ToList();
        var byKey = confirmations
            .Where(c => c.Date == date)
            .GroupBy(c => (c.ScheduleId, c.DoseAt))
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.ConfirmedUtc).First());

        var result = new List<DayDose>();

        foreach (var schedule in scheduleList)
        {
            foreach (var occurrence in OccurrencesOn(schedule, date))
            {
                byKey.TryGetValue((schedule.Id, occurrence.At), out var confirmation);
                result.Add(new DayDose(schedule, occurrence.At, occurrence.WindowMinutes, StatusOf(occurrence, confirmation, localNow), confirmation, IsAsNeeded: false));
            }
        }

        foreach (var schedule in scheduleList.Where(s => s.Kind == ScheduleKind.AsNeeded))
        {
            foreach (var confirmation in byKey.Values.Where(c => c.ScheduleId == schedule.Id))
            {
                result.Add(new DayDose(schedule, confirmation.DoseAt, 0, DoseStatus.Taken, confirmation, IsAsNeeded: true));
            }
        }

        return result.OrderBy(d => d.At).ThenBy(d => d.Schedule.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    /// <summary>The part of the day a dose time belongs to, for the display's sentences.</summary>
    public static DosePart PartOf(TimeOnly at) => at.Hour switch
    {
        < 11 => DosePart.Morning,
        < 14 => DosePart.Midday,
        < 17 => DosePart.Afternoon,
        < 21 => DosePart.Evening,
        _ => DosePart.Night,
    };
}

/// <summary>Part of the day, used to name a dose without a drug name: "your morning medication".</summary>
public enum DosePart
{
    /// <summary>Before 11:00.</summary>
    Morning,

    /// <summary>11:00 to 14:00.</summary>
    Midday,

    /// <summary>14:00 to 17:00.</summary>
    Afternoon,

    /// <summary>17:00 to 21:00.</summary>
    Evening,

    /// <summary>After 21:00.</summary>
    Night,
}
