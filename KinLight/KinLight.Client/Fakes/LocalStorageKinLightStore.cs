using System.Text.Json;
using KinLight.Modules.Display.Client;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Clock.Client;
using Microsoft.JSInterop;

namespace KinLight.Client.Fakes;

/// <summary>
/// Development stand-in for the server: displays live in localStorage, widget data is each widget's sample data.
/// Implements both the display's and the portal's API so a save in the portal shows on the display.
/// Honours the Version token so concurrency behaviour exists from day one. Made-up data only.
/// </summary>
public sealed class LocalStorageKinLightStore : IDisplayApi, IPortalApi
{
    private const string StorageKey = "kinlight.dev.displays";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;
    private readonly IWidgetCatalog _catalog;
    private readonly TimeProvider _clock;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private List<DisplayConfig>? _displays;

    /// <summary>Creates the store.</summary>
    public LocalStorageKinLightStore(IJSRuntime js, IWidgetCatalog catalog, TimeProvider clock)
    {
        _js = js;
        _catalog = catalog;
        _clock = clock;
    }

    /// <inheritdoc />
    public event Func<Task>? ConfigChanged;

    /// <inheritdoc />
    public async Task<DisplayConfig?> GetConfigAsync(CancellationToken cancellationToken)
    {
        var displays = await LoadAsync(cancellationToken);
        return displays.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<object?> GetWidgetDataAsync(Guid placementId, Type dataType, CancellationToken cancellationToken)
    {
        var displays = await LoadAsync(cancellationToken);
        var display = displays.FirstOrDefault(d => d.Widgets.Any(w => w.Id == placementId));
        var placement = display?.Widgets.FirstOrDefault(w => w.Id == placementId);
        if (display is null || placement is null)
        {
            return null;
        }

        var definition = _catalog.Find(placement.TypeKey);
        if (definition?.DataType is null)
        {
            return null;
        }

        var context = new DisplayContext(
            SafeCulture(display.Culture),
            display.FallbackCulture is null ? null : SafeCulture(display.FallbackCulture),
            SafeTimeZone(display.TimeZoneId),
            _clock,
            display.NightMode);

        var sample = definition.CreateSampleData(context);
        return sample is not null && dataType.IsInstanceOfType(sample) ? sample : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DisplaySummary>> ListDisplaysAsync(CancellationToken cancellationToken)
    {
        var displays = await LoadAsync(cancellationToken);
        return displays.Select(d => new DisplaySummary(d.DisplayId, d.Name, d.ScreenShape, d.Culture, d.Widgets.Count, d.Version)).ToList();
    }

    /// <inheritdoc />
    public async Task<DisplayConfig?> GetDisplayAsync(Guid displayId, CancellationToken cancellationToken)
    {
        var displays = await LoadAsync(cancellationToken);
        return displays.FirstOrDefault(d => d.DisplayId == displayId);
    }

    /// <inheritdoc />
    public Task<long> SaveLayoutAsync(SaveDisplayLayoutRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return MutateAsync(request.DisplayId, request.ExpectedVersion, d => d with { Widgets = request.Widgets.ToList() }, cancellationToken);
    }

    /// <inheritdoc />
    public Task<long> UpdateSettingsAsync(UpdateDisplaySettingsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var s = request.Settings;
        return MutateAsync(request.DisplayId, request.ExpectedVersion, d => d with
        {
            Name = s.Name,
            ScreenShape = s.ScreenShape,
            Culture = s.Culture,
            FallbackCulture = s.FallbackCulture,
            TimeZoneId = s.TimeZoneId,
            NightMode = s.NightMode,
        }, cancellationToken);
    }

    private async Task<long> MutateAsync(Guid displayId, long expectedVersion, Func<DisplayConfig, DisplayConfig> change, CancellationToken cancellationToken)
    {
        long version;
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var displays = await LoadUnlockedAsync(cancellationToken);
            var index = displays.FindIndex(d => d.DisplayId == displayId);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Display {displayId} does not exist.");
            }

            var current = displays[index];
            if (current.Version != expectedVersion)
            {
                throw new VersionConflictException(expectedVersion, current.Version);
            }

            var updated = change(current) with { Version = current.Version + 1 };
            displays[index] = updated;
            await PersistAsync(displays, cancellationToken);
            version = updated.Version;
        }
        finally
        {
            _gate.Release();
        }

        // Notify outside the lock: a subscriber (the display page) reloads the config, which takes the lock again.
        await NotifyChangedAsync();
        return version;
    }

    private async Task NotifyChangedAsync()
    {
        if (ConfigChanged is { } handlers)
        {
            foreach (var handler in handlers.GetInvocationList().Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    private async Task<List<DisplayConfig>> LoadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await LoadUnlockedAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<DisplayConfig>> LoadUnlockedAsync(CancellationToken cancellationToken)
    {
        if (_displays is not null)
        {
            return _displays;
        }

        List<DisplayConfig>? loaded = null;
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                loaded = JsonSerializer.Deserialize<List<DisplayConfig>>(json, Json);
            }
        }
        catch (JsonException)
        {
            // Corrupt dev data: start over with the seed.
        }
        catch (JSException)
        {
            // localStorage unavailable: stay in memory for this session.
        }

        _displays = loaded is { Count: > 0 } ? loaded : [Seed()];
        if (loaded is null)
        {
            await PersistAsync(_displays, cancellationToken);
        }

        return _displays;
    }

    private async Task PersistAsync(List<DisplayConfig> displays, CancellationToken cancellationToken)
    {
        _displays = displays;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, JsonSerializer.Serialize(displays, Json));
        }
        catch (JSException)
        {
            // In-memory only for this session.
        }
    }

    private static DisplayConfig Seed() => new(
        DisplayId: Guid.Parse("7f3a1c2e-0000-4000-8000-00000000c0de"),
        Name: "Kitchen",
        Culture: "en",
        FallbackCulture: null,
        TimeZoneId: "Europe/Athens",
        NightMode: NightMode.Default,
        ScreenShape: ScreenShape.Wide16x9,
        Version: 1,
        Widgets:
        [
            new WidgetPlacement(Guid.Parse("7f3a1c2e-0000-4000-8000-000000000001"), ClockWidgetDefinition.Key, X: 0, Y: 0, Width: 12, Height: 2, FontScale: 1.0),
        ]);

    private static System.Globalization.CultureInfo SafeCulture(string name)
    {
        try
        {
            return System.Globalization.CultureInfo.GetCultureInfo(name);
        }
        catch (System.Globalization.CultureNotFoundException)
        {
            return System.Globalization.CultureInfo.CurrentCulture;
        }
    }

    private static TimeZoneInfo SafeTimeZone(string id)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Local;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Local;
        }
    }
}
