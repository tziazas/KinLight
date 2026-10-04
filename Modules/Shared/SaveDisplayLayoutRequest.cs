namespace KinLight.Modules.Shared;

/// <summary>
/// Saves a display's whole layout. Guarded by <paramref name="ExpectedVersion"/> so two portal windows
/// cannot overwrite each other (ARCHITECTURE.md §17).
/// </summary>
/// <param name="DisplayId">The display.</param>
/// <param name="ExpectedVersion">The version the editor loaded. The save fails with <see cref="VersionConflictException"/> if it has moved on.</param>
/// <param name="Widgets">The complete set of placements.</param>
public sealed record SaveDisplayLayoutRequest(Guid DisplayId, long ExpectedVersion, IReadOnlyList<WidgetPlacement> Widgets);
