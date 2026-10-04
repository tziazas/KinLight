using System.Text.Json;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Medication.Client;
using Microsoft.JSInterop;

namespace KinLight.Client.Fakes;

/// <summary>
/// Development stand-in for medication storage: schedules and confirmations in localStorage. Confirmations are stamped
/// with the fake current member. Made-up medications only.
/// </summary>
public sealed class LocalStorageMedicationStore : IMedicationApi
{
    private const string SchedulesKey = "kinlight.dev.medications";
    private const string ConfirmationsKey = "kinlight.dev.doseconfirmations";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;
    private readonly ICurrentMember _member;
    private readonly TimeProvider _clock;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private List<MedicationSchedule>? _schedules;
    private List<DoseConfirmation>? _confirmations;

    /// <summary>Creates the store.</summary>
    public LocalStorageMedicationStore(IJSRuntime js, ICurrentMember member, TimeProvider clock)
    {
        _js = js;
        _member = member;
        _clock = clock;
    }

    /// <inheritdoc />
    public event Func<Task>? Changed;

    /// <inheritdoc />
    public async Task<IReadOnlyList<MedicationSchedule>> ListSchedulesAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await SchedulesAsync(cancellationToken)).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<MedicationSchedule> SaveScheduleAsync(MedicationSchedule schedule, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var schedules = await SchedulesAsync(cancellationToken);
            var index = schedules.FindIndex(s => s.Id == schedule.Id);
            if (index >= 0)
            {
                schedules[index] = schedule;
            }
            else
            {
                schedules.Add(schedule);
            }

            await SaveAsync(SchedulesKey, schedules, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await NotifyAsync();
        return schedule;
    }

    /// <inheritdoc />
    public async Task DeleteScheduleAsync(Guid scheduleId, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var schedules = await SchedulesAsync(cancellationToken);
            schedules.RemoveAll(s => s.Id == scheduleId);
            await SaveAsync(SchedulesKey, schedules, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await NotifyAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DoseConfirmation>> ListConfirmationsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await ConfirmationsAsync(cancellationToken)).Where(c => c.Date >= from && c.Date <= to).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public Task<DoseConfirmation> ConfirmAsync(ConfirmDoseRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return AddConfirmationAsync(request.ScheduleId, request.Date, request.DoseAt, cancellationToken);
    }

    /// <inheritdoc />
    public Task<DoseConfirmation> LogAsNeededAsync(Guid scheduleId, CancellationToken cancellationToken)
    {
        var now = _clock.GetLocalNow().DateTime;
        return AddConfirmationAsync(scheduleId, DateOnly.FromDateTime(now), new TimeOnly(now.Hour, now.Minute), cancellationToken);
    }

    private async Task<DoseConfirmation> AddConfirmationAsync(Guid scheduleId, DateOnly date, TimeOnly doseAt, CancellationToken cancellationToken)
    {
        var confirmation = new DoseConfirmation(Guid.NewGuid(), scheduleId, date, doseAt, _clock.GetUtcNow(), _member.Id, _member.Name);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var confirmations = await ConfirmationsAsync(cancellationToken);
            confirmations.Add(confirmation);
            await SaveAsync(ConfirmationsKey, confirmations, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await NotifyAsync();
        return confirmation;
    }

    private async Task NotifyAsync()
    {
        if (Changed is { } handlers)
        {
            foreach (var handler in handlers.GetInvocationList().Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    private async Task<List<MedicationSchedule>> SchedulesAsync(CancellationToken cancellationToken)
        => _schedules ??= await LoadAsync<MedicationSchedule>(SchedulesKey, cancellationToken) ?? Seed();

    private async Task<List<DoseConfirmation>> ConfirmationsAsync(CancellationToken cancellationToken)
        => _confirmations ??= await LoadAsync<DoseConfirmation>(ConfirmationsKey, cancellationToken) ?? [];

    private async Task<List<T>?> LoadAsync<T>(string key, CancellationToken cancellationToken)
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<List<T>>(json, Json);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (JSException)
        {
            return null;
        }
    }

    private async Task SaveAsync<T>(string key, List<T> items, CancellationToken cancellationToken)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(items, Json));
        }
        catch (JSException)
        {
        }
    }

    // One made-up example so the Today page and the display have something to show.
    private static List<MedicationSchedule> Seed()
    {
        var instructions = LocalizedText.From("en", "One white tablet with a glass of water.");
        instructions.SetReviewed("es", "Un comprimido blanco con un vaso de agua.");
        instructions.SetReviewed("el", "Ένα άσπρο χάπι με ένα ποτήρι νερό.");

        return
        [
            new MedicationSchedule
            {
                Id = Guid.Parse("00000000-0000-4000-8000-00000000ed01"),
                Name = "Example tablets 5 mg",
                Instructions = instructions,
                Kind = ScheduleKind.Daily,
                Doses = [new DoseTime(new TimeOnly(8, 0), 90), new DoseTime(new TimeOnly(20, 0), 90)],
            },
        ];
    }
}
