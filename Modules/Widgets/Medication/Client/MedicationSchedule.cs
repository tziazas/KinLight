using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>How often a medication is due.</summary>
public enum ScheduleKind
{
    /// <summary>Every day at the dose times.</summary>
    Daily = 0,

    /// <summary>On the selected weekdays at the dose times.</summary>
    Weekly = 1,

    /// <summary>Every N days from an anchor date, at the dose times.</summary>
    EveryNDays = 2,

    /// <summary>Only on specific dates, at the dose times.</summary>
    SpecificDates = 3,

    /// <summary>When needed. No reminders; the caregiver logs each dose as given.</summary>
    AsNeeded = 4,
}

/// <summary>One dose time in a day.</summary>
/// <param name="At">Local wall-clock time the dose is due.</param>
/// <param name="WindowMinutes">How long after <paramref name="At"/> the dose may still be given before it counts as missed.</param>
public sealed record DoseTime(TimeOnly At, int WindowMinutes = 60);

/// <summary>
/// A medication and when it is due. Times are wall-clock, interpreted in the household's time zone. The name is for
/// the portal only; the display shows the instructions, which are safety-relevant text and never an unreviewed translation.
/// </summary>
public sealed record MedicationSchedule
{
    /// <summary>The schedule id.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Medication name and dose as the family writes it, e.g. "Example tablets 5 mg". Portal only.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>What the person should do, shown on the display, e.g. "Two white tablets with water."</summary>
    public LocalizedText Instructions { get; init; } = new();

    /// <summary>How often.</summary>
    public ScheduleKind Kind { get; init; } = ScheduleKind.Daily;

    /// <summary>Dose times per applicable day. Empty for <see cref="ScheduleKind.AsNeeded"/>.</summary>
    public IReadOnlyList<DoseTime> Doses { get; init; } = [];

    /// <summary>Weekdays for <see cref="ScheduleKind.Weekly"/>.</summary>
    public IReadOnlyList<DayOfWeek> Weekdays { get; init; } = [];

    /// <summary>Interval for <see cref="ScheduleKind.EveryNDays"/>.</summary>
    public int EveryNDays { get; init; } = 2;

    /// <summary>First day for <see cref="ScheduleKind.EveryNDays"/>.</summary>
    public DateOnly? AnchorDate { get; init; }

    /// <summary>Dates for <see cref="ScheduleKind.SpecificDates"/>.</summary>
    public IReadOnlyList<DateOnly> Dates { get; init; } = [];

    /// <summary>Optional first day the schedule applies.</summary>
    public DateOnly? StartsOn { get; init; }

    /// <summary>Optional last day the schedule applies.</summary>
    public DateOnly? EndsOn { get; init; }

    /// <summary>Inactive schedules produce no doses but keep their history.</summary>
    public bool IsActive { get; init; } = true;
}

/// <summary>A dose that was given, recorded by the caregiver who gave it (never by the person).</summary>
/// <param name="Id">The confirmation id.</param>
/// <param name="ScheduleId">The medication.</param>
/// <param name="Date">The local date of the dose.</param>
/// <param name="DoseAt">The scheduled time, or for as-needed medication the time it was given.</param>
/// <param name="ConfirmedUtc">When it was confirmed.</param>
/// <param name="ConfirmedByMemberId">Who confirmed.</param>
/// <param name="ConfirmedByName">Their name at the time, kept so history stays readable if they leave.</param>
public sealed record DoseConfirmation(
    Guid Id,
    Guid ScheduleId,
    DateOnly Date,
    TimeOnly DoseAt,
    DateTimeOffset ConfirmedUtc,
    Guid ConfirmedByMemberId,
    string ConfirmedByName);
