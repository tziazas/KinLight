namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>A size on the display grid, in columns and rows.</summary>
/// <param name="Width">Width in grid columns.</param>
/// <param name="Height">Height in grid rows.</param>
public readonly record struct WidgetSize(int Width, int Height);
