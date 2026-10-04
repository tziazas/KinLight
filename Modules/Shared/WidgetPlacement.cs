namespace KinLight.Modules.Shared;

/// <summary>
/// One widget placed on a display's grid (ARCHITECTURE.md §10). Sent to the display as part of <see cref="DisplayConfig"/>.
/// </summary>
/// <param name="Id">The placement id.</param>
/// <param name="TypeKey">The widget type's stable key, e.g. "kinlight.clock".</param>
/// <param name="X">Column, 0-based, within <see cref="BoardGrid.Columns"/>.</param>
/// <param name="Y">Row, 0-based, within <see cref="BoardGrid.Rows"/>.</param>
/// <param name="Width">Width in columns.</param>
/// <param name="Height">Height in rows.</param>
/// <param name="FontScale">Text size multiplier for this widget; 1.0 is the default.</param>
/// <param name="HighContrast">Whether the widget renders in high contrast.</param>
/// <param name="IsEnabled">Whether the widget is shown at all.</param>
/// <param name="VisibleFrom">Optional local time the widget becomes visible each day.</param>
/// <param name="VisibleTo">Optional local time the widget stops being visible each day.</param>
/// <param name="SettingsJson">The widget's settings, serialized. Deserialized by the widget's settings type; bad JSON falls back to defaults.</param>
public sealed record WidgetPlacement(
    Guid Id,
    string TypeKey,
    int X,
    int Y,
    int Width,
    int Height,
    double FontScale = 1.0,
    bool HighContrast = false,
    bool IsEnabled = true,
    TimeOnly? VisibleFrom = null,
    TimeOnly? VisibleTo = null,
    string? SettingsJson = null);
