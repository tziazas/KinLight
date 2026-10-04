namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>Marker type and keys for the photos widget's <c>.resx</c> strings (ARCHITECTURE.md §15).</summary>
public sealed class PhotosStrings
{
    /// <summary>The widget's name.</summary>
    public const string WidgetName = nameof(WidgetName);

    /// <summary>Settings label: album.</summary>
    public const string Settings_Album = nameof(Settings_Album);

    /// <summary>Settings label: seconds per photo.</summary>
    public const string Settings_IntervalSeconds = nameof(Settings_IntervalSeconds);

    /// <summary>Settings label: show captions.</summary>
    public const string Settings_ShowCaptions = nameof(Settings_ShowCaptions);

    /// <summary>Settings label: caption placement.</summary>
    public const string Settings_CaptionPlacement = nameof(Settings_CaptionPlacement);

    /// <summary>Settings label: shuffle.</summary>
    public const string Settings_Shuffle = nameof(Settings_Shuffle);

    /// <summary>Caption placement option: over the photo.</summary>
    public const string CaptionPlacement_Overlay = nameof(CaptionPlacement_Overlay);

    /// <summary>Caption placement option: under the photo.</summary>
    public const string CaptionPlacement_Below = nameof(CaptionPlacement_Below);

    private PhotosStrings()
    {
    }
}
