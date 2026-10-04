using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Portal.Client.Settings;

/// <summary>
/// Custom editors for individual widget settings properties, e.g. an album picker for the photos widget's Album.
/// The generated settings form uses the registered component instead of the default field for that property.
/// An editor component must have parameters <c>object? Value</c>, <c>EventCallback&lt;object?&gt; ValueChanged</c>
/// and <c>string Label</c>.
/// </summary>
public interface IWidgetSettingsEditorRegistry
{
    /// <summary>The editor for a property, or null to use the default field.</summary>
    Type? Find(Type settingsType, string propertyName);
}

internal sealed class WidgetSettingsEditorRegistry : IWidgetSettingsEditorRegistry
{
    private readonly Dictionary<(Type, string), Type> _editors;

    public WidgetSettingsEditorRegistry(IEnumerable<WidgetSettingsEditorRegistration> registrations)
    {
        _editors = registrations.ToDictionary(r => (r.SettingsType, r.PropertyName), r => r.EditorComponent);
    }

    public Type? Find(Type settingsType, string propertyName)
        => _editors.TryGetValue((settingsType, propertyName), out var editor) ? editor : null;
}

/// <summary>One registration; see <see cref="PortalSettingsServiceCollectionExtensions.AddWidgetSettingsEditor{TSettings}"/>.</summary>
public sealed record WidgetSettingsEditorRegistration(Type SettingsType, string PropertyName, Type EditorComponent);

/// <summary>Registration helpers.</summary>
public static class PortalSettingsServiceCollectionExtensions
{
    /// <summary>Uses <paramref name="editorComponent"/> for <paramref name="propertyName"/> of <typeparamref name="TSettings"/>.</summary>
    public static IServiceCollection AddWidgetSettingsEditor<TSettings>(this IServiceCollection services, string propertyName, Type editorComponent)
        where TSettings : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(editorComponent);
        if (!typeof(IComponent).IsAssignableFrom(editorComponent))
        {
            throw new ArgumentException("Editor must be a Blazor component.", nameof(editorComponent));
        }

        services.AddSingleton(new WidgetSettingsEditorRegistration(typeof(TSettings), propertyName, editorComponent));
        return services;
    }

    /// <summary>Adds a contributed navigation item.</summary>
    public static IServiceCollection AddPortalNavItem<TItem>(this IServiceCollection services)
        where TItem : class, IPortalNavItem
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IPortalNavItem, TItem>();
        return services;
    }
}
