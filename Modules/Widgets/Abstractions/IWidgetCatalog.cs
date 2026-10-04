namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>The registered widget definitions, looked up by type key.</summary>
public interface IWidgetCatalog
{
    /// <summary>All registered widgets, in registration order.</summary>
    IReadOnlyList<IWidgetDefinition> All { get; }

    /// <summary>Finds a widget by type key, or null when no such widget is registered (the display then renders nothing).</summary>
    IWidgetDefinition? Find(string typeKey);
}

internal sealed class WidgetCatalog : IWidgetCatalog
{
    private readonly Dictionary<string, IWidgetDefinition> _byKey;

    public WidgetCatalog(IEnumerable<IWidgetDefinition> definitions)
    {
        All = definitions.ToList();
        _byKey = All.ToDictionary(d => d.TypeKey, StringComparer.Ordinal);
    }

    public IReadOnlyList<IWidgetDefinition> All { get; }

    public IWidgetDefinition? Find(string typeKey)
        => typeKey is not null && _byKey.TryGetValue(typeKey, out var definition) ? definition : null;
}
