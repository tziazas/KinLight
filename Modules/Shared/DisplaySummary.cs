namespace KinLight.Modules.Shared;

/// <summary>A display as listed in the portal.</summary>
/// <param name="Id">The display id.</param>
/// <param name="Name">The display's name.</param>
/// <param name="ScreenShape">The screen's aspect ratio.</param>
/// <param name="Culture">The display language.</param>
/// <param name="WidgetCount">How many widgets are placed.</param>
/// <param name="Version">Concurrency token.</param>
public sealed record DisplaySummary(Guid Id, string Name, ScreenShape ScreenShape, string Culture, int WidgetCount, long Version);
