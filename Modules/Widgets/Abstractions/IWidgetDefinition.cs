namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Describes a widget type to the display, the portal and the server (ARCHITECTURE.md §14).
/// Registered explicitly with <see cref="WidgetServiceCollectionExtensions.AddWidget{TDefinition}"/>, never by assembly scanning.
/// </summary>
public interface IWidgetDefinition
{
    /// <summary>
    /// Stable key stored in the database with every placement, e.g. "kinlight.clock".
    /// Never changed after release.
    /// </summary>
    string TypeKey { get; }

    /// <summary>Resource key of the widget's human-readable name in <see cref="StringsType"/>.</summary>
    string NameResourceKey { get; }

    /// <summary>Marker type for the widget's <c>.resx</c> strings, resolved per display language through <see cref="WidgetStrings"/>.</summary>
    Type StringsType { get; }

    /// <summary>The settings class. Deserialized from <c>WidgetPlacement.SettingsJson</c>; must have a parameterless constructor with sensible defaults.</summary>
    Type SettingsType { get; }

    /// <summary>The Razor component that renders the widget on the display.</summary>
    Type ViewComponent { get; }

    /// <summary>The server-fetched data type the view receives, or null when the widget needs no outside data.</summary>
    Type? DataType { get; }

    /// <summary>Size used when the widget is first added to a layout.</summary>
    WidgetSize DefaultSize { get; }

    /// <summary>Smallest size the layout editor allows.</summary>
    WidgetSize MinSize { get; }

    /// <summary>
    /// Made-up data for previews in the layout editor and for development hosts. Never real people
    /// (ARCHITECTURE.md §2, rule 7). Returns null for widgets without data.
    /// </summary>
    object? CreateSampleData(DisplayContext context) => null;
}
