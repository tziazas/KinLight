using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>
/// What the display receives: today's scheduled doses with their instructions and, when confirmed, the time.
/// The display works out due/taken from its own clock, so it needs no refetch per minute. Drug names are not sent.
/// </summary>
/// <param name="Date">The local date this covers.</param>
/// <param name="Doses">The doses, in time order.</param>
public sealed record MedicationData(DateOnly Date, IReadOnlyList<DoseSlot> Doses)
{
    /// <summary>Nothing scheduled.</summary>
    public static MedicationData Empty(DateOnly date) => new(date, []);
}

/// <summary>One scheduled dose on the display.</summary>
/// <param name="ScheduleId">The medication.</param>
/// <param name="At">Local time due.</param>
/// <param name="WindowMinutes">Window length.</param>
/// <param name="Instructions">What to do; safety-relevant, resolved with unreviewed translations disallowed.</param>
/// <param name="TakenAt">Local time it was confirmed, or null.</param>
public sealed record DoseSlot(Guid ScheduleId, TimeOnly At, int WindowMinutes, LocalizedText Instructions, TimeOnly? TakenAt);
