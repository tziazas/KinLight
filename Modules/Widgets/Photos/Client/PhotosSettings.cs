using System.ComponentModel.DataAnnotations;

namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>Where a photo's caption is shown.</summary>
public enum CaptionPlacement
{
    /// <summary>Over the bottom of the photo, on a dark band.</summary>
    [Display(Name = nameof(PhotosStrings.CaptionPlacement_Overlay))]
    Overlay = 0,

    /// <summary>In a row underneath the photo.</summary>
    [Display(Name = nameof(PhotosStrings.CaptionPlacement_Below))]
    Below = 1,
}

/// <summary>
/// Settings for the photos widget. The portal generates its form from these annotations; <c>Display.Name</c> values
/// are keys in <see cref="PhotosStrings"/>, resolved in the portal's language.
/// </summary>
public sealed class PhotosSettings
{
    /// <summary>Album to show; null or empty means every photo in the household library.</summary>
    [Display(Name = nameof(PhotosStrings.Settings_Album))]
    public string? Album { get; set; }

    /// <summary>Seconds each photo stays. Slow by default: sudden changes unsettle.</summary>
    [Display(Name = nameof(PhotosStrings.Settings_IntervalSeconds))]
    [Range(10, 600)]
    public int IntervalSeconds { get; set; } = 45;

    /// <summary>Whether captions are shown at all.</summary>
    [Display(Name = nameof(PhotosStrings.Settings_ShowCaptions))]
    public bool ShowCaptions { get; set; } = true;

    /// <summary>Where the caption goes.</summary>
    [Display(Name = nameof(PhotosStrings.Settings_CaptionPlacement))]
    public CaptionPlacement CaptionPlacement { get; set; } = CaptionPlacement.Overlay;

    /// <summary>Random order instead of upload order.</summary>
    [Display(Name = nameof(PhotosStrings.Settings_Shuffle))]
    public bool Shuffle { get; set; }
}
