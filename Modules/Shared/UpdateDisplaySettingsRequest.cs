namespace KinLight.Modules.Shared;

/// <summary>Updates a display's settings, guarded by <paramref name="ExpectedVersion"/>.</summary>
/// <param name="DisplayId">The display.</param>
/// <param name="ExpectedVersion">The version the form loaded.</param>
/// <param name="Settings">The new settings.</param>
public sealed record UpdateDisplaySettingsRequest(Guid DisplayId, long ExpectedVersion, DisplaySettings Settings);
