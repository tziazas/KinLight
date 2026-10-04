using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Medication.Client;

namespace KinLight.Modules.Widgets.Medication.Api;

/// <summary>
/// Finds doses whose window has just closed without confirmation, so the host can send a
/// <see cref="NotificationKind.DoseMissed"/> notification to Owners and Family. Pure: the host supplies the
/// set of doses it already alerted about and persists what this returns.
/// </summary>
public static class MissedDoseDetector
{
    /// <summary>A dose key that is stable across runs.</summary>
    /// <param name="ScheduleId">The medication.</param>
    /// <param name="Date">Local date.</param>
    /// <param name="At">Scheduled time.</param>
    public sealed record DoseKey(Guid ScheduleId, DateOnly Date, TimeOnly At);

    /// <summary>Missed doses on <paramref name="date"/> that are not in <paramref name="alreadyAlerted"/>.</summary>
    public static IReadOnlyList<DoseKey> NewlyMissed(IEnumerable<MedicationSchedule> schedules, IEnumerable<DoseConfirmation> confirmations, DateOnly date, DateTime localNow, IReadOnlySet<DoseKey> alreadyAlerted)
    {
        ArgumentNullException.ThrowIfNull(alreadyAlerted);

        return DoseLogic.BuildDay(schedules, confirmations, date, localNow)
            .Where(d => d.Status == DoseStatus.Missed)
            .Select(d => new DoseKey(d.Schedule.Id, date, d.At))
            .Where(k => !alreadyAlerted.Contains(k))
            .ToList();
    }

    /// <summary>Due doses on <paramref name="date"/> not in <paramref name="alreadyAlerted"/>, for <see cref="NotificationKind.DoseDue"/> reminders to caregivers.</summary>
    public static IReadOnlyList<DoseKey> NewlyDue(IEnumerable<MedicationSchedule> schedules, IEnumerable<DoseConfirmation> confirmations, DateOnly date, DateTime localNow, IReadOnlySet<DoseKey> alreadyAlerted)
    {
        ArgumentNullException.ThrowIfNull(alreadyAlerted);

        return DoseLogic.BuildDay(schedules, confirmations, date, localNow)
            .Where(d => d.Status == DoseStatus.Due)
            .Select(d => new DoseKey(d.Schedule.Id, date, d.At))
            .Where(k => !alreadyAlerted.Contains(k))
            .ToList();
    }
}
