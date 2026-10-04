using System.Globalization;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>Resolves a widget's human-readable name from its strings resource.</summary>
public static class WidgetNames
{
    /// <summary>The localized name of <paramref name="definition"/> in <paramref name="culture"/>, falling back to the resource key.</summary>
    public static string NameOf(IWidgetDefinition definition, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return WidgetStrings.For(definition.StringsType, culture)[definition.NameResourceKey];
    }
}
