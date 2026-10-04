using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Built-in widget text for an explicit culture (ARCHITECTURE.md §15). The display's language is data from its
/// configuration, never the process culture, so widgets resolve strings through <see cref="DisplayContext.Culture"/>
/// rather than <c>IStringLocalizer</c>. Each resource is a whole sentence for one case.
/// </summary>
public interface IWidgetStrings
{
    /// <summary>The culture strings are resolved for.</summary>
    CultureInfo Culture { get; }

    /// <summary>The sentence for <paramref name="key"/>, or the key itself when no resource exists in any language.</summary>
    string this[string key] { get; }

    /// <summary>The sentence for <paramref name="key"/> formatted with <paramref name="args"/> using the strings' culture.</summary>
    string this[string key, params object[] args] { get; }
}

/// <summary>Creates <see cref="IWidgetStrings"/> for a strings marker type and a culture.</summary>
public static class WidgetStrings
{
    private static readonly ConcurrentDictionary<Type, ResourceManager> Managers = new();

    /// <summary>Strings of <typeparamref name="TStrings"/> for <paramref name="culture"/>.</summary>
    public static IWidgetStrings For<TStrings>(CultureInfo culture) => For(typeof(TStrings), culture);

    /// <summary>Strings of <paramref name="stringsType"/> (a widget definition's <c>StringsType</c>) for <paramref name="culture"/>.</summary>
    public static IWidgetStrings For(Type stringsType, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(stringsType);
        ArgumentNullException.ThrowIfNull(culture);

        var manager = Managers.GetOrAdd(stringsType, static t => new ResourceManager(t));
        return new ResourceWidgetStrings(manager, culture);
    }

    private sealed class ResourceWidgetStrings(ResourceManager manager, CultureInfo culture) : IWidgetStrings
    {
        public CultureInfo Culture { get; } = culture;

        public string this[string key] => Get(key);

        public string this[string key, params object[] args] => string.Format(Culture, Get(key), args);

        private string Get(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            try
            {
                return manager.GetString(key, Culture) ?? key;
            }
            catch (MissingManifestResourceException)
            {
                return key;
            }
            catch (MissingSatelliteAssemblyException)
            {
                return manager.GetString(key, CultureInfo.InvariantCulture) ?? key;
            }
        }
    }
}
