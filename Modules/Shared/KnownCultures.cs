namespace KinLight.Modules.Shared;

/// <summary>
/// Languages offered to families. A language is listed only when every resource file for that side is fully
/// translated; <c>KinLight.Localization.Tests</c> fails otherwise (ARCHITECTURE.md §15). Add a language by translating
/// the <c>.resx</c> files and then adding it here.
/// </summary>
public static class KnownCultures
{
    /// <summary>Languages a display can be set to (widgets and display shell fully translated).</summary>
    public static IReadOnlyList<string> Display { get; } = ["en", "es", "el"];

    /// <summary>Languages the portal is available in (portal fully translated).</summary>
    public static IReadOnlyList<string> Portal { get; } = ["en", "el"];
}
