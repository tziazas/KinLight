using KinLight.Modules.Widgets.Abstractions;

namespace KinLight.Modules.Widgets.Clock.Client;

/// <summary>The clock and day widget.</summary>
public sealed class ClockWidgetDefinition : IWidgetDefinition
{
    /// <summary>The stable type key. Never change after release.</summary>
    public const string Key = "kinlight.clock";

    /// <inheritdoc />
    public string TypeKey => Key;

    /// <inheritdoc />
    public string NameResourceKey => nameof(ClockStrings.WidgetName);

    /// <inheritdoc />
    public Type StringsType => typeof(ClockStrings);

    /// <inheritdoc />
    public Type SettingsType => typeof(ClockSettings);

    /// <inheritdoc />
    public Type ViewComponent => typeof(ClockWidget);

    /// <inheritdoc />
    public Type? DataType => null;

    /// <inheritdoc />
    public WidgetSize DefaultSize => new(6, 2);

    /// <inheritdoc />
    public WidgetSize MinSize => new(4, 1);
}
