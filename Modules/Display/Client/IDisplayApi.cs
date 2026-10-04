using KinLight.Modules.Shared;

namespace KinLight.Modules.Display.Client;

/// <summary>
/// What the display needs from the server (ARCHITECTURE.md §11). Implemented over HTTP with the paired
/// device token in production, and by an in-browser fake in development hosts.
/// </summary>
public interface IDisplayApi
{
    /// <summary>
    /// Loads the display's configuration. Returns null when nothing is available (no server and no cached copy),
    /// in which case the display shows a clock-only layout.
    /// </summary>
    Task<DisplayConfig?> GetConfigAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Loads the last good data for a placement as an instance of <paramref name="dataType"/>, or null if none exists.
    /// Never throws for missing data; a failed fetch keeps the previous data.
    /// </summary>
    Task<object?> GetWidgetDataAsync(Guid placementId, Type dataType, CancellationToken cancellationToken);

    /// <summary>Raised when the server signals that the configuration changed. The display then calls <see cref="GetConfigAsync"/> again.</summary>
    event Func<Task>? ConfigChanged;
}
