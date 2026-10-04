using KinLight.Modules.Shared;

namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>What the display shows: the slides for one placement, already filtered by album.</summary>
/// <param name="Slides">The slides in library order.</param>
public sealed record PhotosData(IReadOnlyList<PhotoSlide> Slides)
{
    /// <summary>No photos.</summary>
    public static PhotosData Empty { get; } = new([]);
}

/// <summary>One photo on the display.</summary>
/// <param name="Id">The photo id.</param>
/// <param name="ImageUrl">Where the display loads the image from. The host decides how this is authorized.</param>
/// <param name="Caption">Who this is to the viewer, per language. Null when none was entered.</param>
public sealed record PhotoSlide(Guid Id, string ImageUrl, LocalizedText? Caption);
