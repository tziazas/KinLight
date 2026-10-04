using KinLight.Modules.Widgets.Medication.Client;

namespace KinLight.Modules.Widgets.Medication.Api;

/// <summary>Household medication storage, implemented by the host. Every call is scoped to a household.</summary>
public interface IMedicationStore
{
    /// <summary>All schedules of a household.</summary>
    Task<IReadOnlyList<MedicationSchedule>> ListSchedulesAsync(Guid householdId, CancellationToken cancellationToken);

    /// <summary>Confirmations of a household between two local dates, inclusive.</summary>
    Task<IReadOnlyList<DoseConfirmation>> ListConfirmationsAsync(Guid householdId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
}
