using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Medication.Client;

namespace KinLight.Modules.Widgets.Medication.Api;

/// <summary>Builds today's dose slots for a display, in the display's time zone, without drug names.</summary>
public sealed class MedicationDataProvider : IWidgetDataProvider
{
    private readonly IMedicationStore _store;
    private readonly TimeProvider _clock;

    /// <summary>Creates the provider.</summary>
    public MedicationDataProvider(IMedicationStore store, TimeProvider clock)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <inheritdoc />
    public string TypeKey => MedicationWidgetDefinition.Key;

    /// <inheritdoc />
    public TimeSpan RefreshInterval => TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    public async Task<object> FetchAsync(WidgetDataRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
        var localNow = TimeZoneInfo.ConvertTime(_clock.GetUtcNow(), timeZone).DateTime;
        var today = DateOnly.FromDateTime(localNow);

        var schedules = await _store.ListSchedulesAsync(request.HouseholdId, cancellationToken).ConfigureAwait(false);
        var confirmations = await _store.ListConfirmationsAsync(request.HouseholdId, today, today, cancellationToken).ConfigureAwait(false);

        return Build(schedules, confirmations, today, localNow, timeZone);
    }

    /// <summary>Pure projection of a day's doses into display slots. Shared with development hosts.</summary>
    public static MedicationData Build(IEnumerable<MedicationSchedule> schedules, IEnumerable<DoseConfirmation> confirmations, DateOnly date, DateTime localNow, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var slots = DoseLogic.BuildDay(schedules, confirmations, date, localNow)
            .Where(d => !d.IsAsNeeded)
            .Select(d => new DoseSlot(
                d.Schedule.Id,
                d.At,
                d.WindowMinutes,
                d.Schedule.Instructions,
                d.Confirmation is null ? null : TimeOnly.FromDateTime(TimeZoneInfo.ConvertTime(d.Confirmation.ConfirmedUtc, timeZone).DateTime)))
            .ToList();

        return new MedicationData(date, slots);
    }
}
