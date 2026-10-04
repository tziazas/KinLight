namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>Confirms a scheduled dose. The server stamps who and when from the signed-in member.</summary>
/// <param name="ScheduleId">The medication.</param>
/// <param name="Date">Local date of the dose.</param>
/// <param name="DoseAt">Scheduled time.</param>
public sealed record ConfirmDoseRequest(Guid ScheduleId, DateOnly Date, TimeOnly DoseAt);

/// <summary>
/// The portal's view of medications (ARCHITECTURE.md §13). Implemented over HTTP in production and by an in-browser
/// fake in development hosts. Scoped to the signed-in member's household; confirming is allowed for every role,
/// editing schedules for Owner and Family only.
/// </summary>
public interface IMedicationApi
{
    /// <summary>All schedules, active and inactive.</summary>
    Task<IReadOnlyList<MedicationSchedule>> ListSchedulesAsync(CancellationToken cancellationToken);

    /// <summary>Creates or updates a schedule.</summary>
    Task<MedicationSchedule> SaveScheduleAsync(MedicationSchedule schedule, CancellationToken cancellationToken);

    /// <summary>Deletes a schedule. Its confirmations stay in the history.</summary>
    Task DeleteScheduleAsync(Guid scheduleId, CancellationToken cancellationToken);

    /// <summary>Confirmations between two local dates, inclusive.</summary>
    Task<IReadOnlyList<DoseConfirmation>> ListConfirmationsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);

    /// <summary>Confirms a scheduled dose as given by the signed-in member, now.</summary>
    Task<DoseConfirmation> ConfirmAsync(ConfirmDoseRequest request, CancellationToken cancellationToken);

    /// <summary>Logs an as-needed dose as given by the signed-in member, now.</summary>
    Task<DoseConfirmation> LogAsNeededAsync(Guid scheduleId, CancellationToken cancellationToken);

    /// <summary>Raised after any change, so Today and the display refresh.</summary>
    event Func<Task>? Changed;
}
