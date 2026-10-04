using System.Globalization;
using System.Text.Json.Serialization;

namespace KinLight.Modules.Shared;

/// <summary>
/// Text a family member or caregiver entered, in one or more languages (ARCHITECTURE.md §15).
/// Holds a map of culture name to text, plus the set of cultures whose text was machine-translated
/// and not yet reviewed by a person. Resolution walks the requested culture's parents, then the
/// fallback culture and its parents, then any reviewed value.
/// </summary>
public sealed class LocalizedText
{
    /// <summary>Text by culture name (e.g. "en", "en-GB", "es"). Keys are compared case-insensitively.</summary>
    [JsonInclude]
    public Dictionary<string, string> Values { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Cultures in <see cref="Values"/> that were machine-translated and have not been reviewed.</summary>
    [JsonInclude]
    public HashSet<string> Unreviewed { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>True when there is no text in any language.</summary>
    [JsonIgnore]
    public bool IsEmpty => Values.Count == 0;

    /// <summary>Creates text in a single language, written by a person.</summary>
    public static LocalizedText From(string culture, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);
        ArgumentNullException.ThrowIfNull(text);

        var result = new LocalizedText();
        result.Values[culture] = text;
        return result;
    }

    /// <summary>
    /// Resolves the best text for <paramref name="culture"/>.
    /// </summary>
    /// <param name="culture">The display language.</param>
    /// <param name="fallbackCulture">The display's backup language, if any.</param>
    /// <param name="allowUnreviewed">
    /// False for safety-relevant text (medication instructions): unreviewed machine translations are skipped
    /// and the text falls back to a language a person wrote or reviewed.
    /// </param>
    /// <returns>The resolved text, or null when nothing acceptable exists.</returns>
    public string? Resolve(CultureInfo culture, CultureInfo? fallbackCulture = null, bool allowUnreviewed = true)
    {
        ArgumentNullException.ThrowIfNull(culture);

        return ResolveChain(culture, allowUnreviewed)
            ?? (fallbackCulture is null ? null : ResolveChain(fallbackCulture, allowUnreviewed))
            ?? Values.FirstOrDefault(pair => IsAcceptable(pair.Key, allowUnreviewed)).Value;
    }

    /// <summary>Sets the text for a culture as written or reviewed by a person.</summary>
    public void SetReviewed(string culture, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);
        ArgumentNullException.ThrowIfNull(text);

        Values[culture] = text;
        Unreviewed.Remove(culture);
    }

    /// <summary>
    /// Sets a machine translation for a culture. Never overwrites text a person wrote or reviewed.
    /// </summary>
    /// <returns>True if the translation was stored.</returns>
    public bool SetMachineTranslated(string culture, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);
        ArgumentNullException.ThrowIfNull(text);

        if (Values.ContainsKey(culture) && !Unreviewed.Contains(culture))
        {
            return false;
        }

        Values[culture] = text;
        Unreviewed.Add(culture);
        return true;
    }

    private string? ResolveChain(CultureInfo culture, bool allowUnreviewed)
    {
        for (var current = culture; !string.IsNullOrEmpty(current.Name); current = current.Parent)
        {
            if (Values.TryGetValue(current.Name, out var text) && IsAcceptable(current.Name, allowUnreviewed))
            {
                return text;
            }
        }

        return null;
    }

    private bool IsAcceptable(string culture, bool allowUnreviewed) => allowUnreviewed || !Unreviewed.Contains(culture);
}
