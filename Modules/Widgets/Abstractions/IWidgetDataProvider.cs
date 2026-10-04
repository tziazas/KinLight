namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Fetches a widget's outside data on the server (ARCHITECTURE.md §14). Implemented in a widget's Api project.
/// The host runs providers on a schedule, keeps the last good result, and pings the display.
/// A failed fetch throws; the host keeps the previous data.
/// </summary>
public interface IWidgetDataProvider
{
    /// <summary>The widget type this provider fetches data for.</summary>
    string TypeKey { get; }

    /// <summary>How often the host should refresh this widget's data.</summary>
    TimeSpan RefreshInterval { get; }

    /// <summary>
    /// Fetches fresh data for one placement.
    /// </summary>
    /// <param name="request">The placement and display being fetched for.</param>
    /// <param name="cancellationToken">Cancellation.</param>
    /// <returns>An instance of the widget definition's <c>DataType</c>.</returns>
    Task<object> FetchAsync(WidgetDataRequest request, CancellationToken cancellationToken);
}

/// <summary>What a data provider needs to know about the placement it is fetching for.</summary>
/// <param name="HouseholdId">The household that owns the display. Every fetch is scoped to one household.</param>
/// <param name="DisplayId">The display.</param>
/// <param name="PlacementId">The placement.</param>
/// <param name="SettingsJson">The placement's settings JSON.</param>
/// <param name="TimeZoneId">The display's IANA time zone.</param>
/// <param name="Culture">The display language.</param>
public sealed record WidgetDataRequest(
    Guid HouseholdId,
    Guid DisplayId,
    Guid PlacementId,
    string? SettingsJson,
    string TimeZoneId,
    string Culture);
