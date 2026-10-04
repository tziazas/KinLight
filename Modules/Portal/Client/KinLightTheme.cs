using MudBlazor;

namespace KinLight.Modules.Portal.Client;

/// <summary>
/// The portal's MudBlazor theme (ARCHITECTURE.md §13, Visual design):
/// harbor blue primary, sage secondary, amber for unsaved changes, cool off-white background,
/// Atkinson Hyperlegible Next.
/// </summary>
public static class KinLightTheme
{
    /// <summary>Harbor blue. Primary actions and navigation.</summary>
    public const string HarborBlue = "#2F5D7C";

    /// <summary>Sage. Secondary accents and confirmations.</summary>
    public const string Sage = "#5E7F63";

    /// <summary>Amber. Unsaved changes.</summary>
    public const string Amber = "#B7791F";

    /// <summary>Cool off-white page background.</summary>
    public const string OffWhite = "#F6F8FA";

    private static readonly string[] FontFamily =
    [
        "Atkinson Hyperlegible Next",
        "Noto Sans",
        "system-ui",
        "sans-serif",
    ];

    /// <summary>The theme to pass to <c>MudThemeProvider</c>.</summary>
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = HarborBlue,
            Secondary = Sage,
            Tertiary = Amber,
            Warning = Amber,
            Background = OffWhite,
            AppbarBackground = HarborBlue,
            DrawerBackground = "#FFFFFF",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#8FB4D1",
            Secondary = "#9DBBA2",
            Tertiary = "#E0B15C",
            Warning = "#E0B15C",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = FontFamily },
            // No all-caps buttons: calmer, and CSS uppercasing keeps Greek accents on capitals, which is wrong in Greek.
            Button = new ButtonTypography { FontFamily = FontFamily, TextTransform = "none", LetterSpacing = "normal" },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
        },
    };
}
