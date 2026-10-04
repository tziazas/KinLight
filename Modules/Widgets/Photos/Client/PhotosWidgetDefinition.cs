using System.Globalization;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;

namespace KinLight.Modules.Widgets.Photos.Client;

/// <summary>The photos widget.</summary>
public sealed class PhotosWidgetDefinition : IWidgetDefinition
{
    /// <summary>The stable type key. Never change after release.</summary>
    public const string Key = "kinlight.photos";

    /// <inheritdoc />
    public string TypeKey => Key;

    /// <inheritdoc />
    public string NameResourceKey => nameof(PhotosStrings.WidgetName);

    /// <inheritdoc />
    public Type StringsType => typeof(PhotosStrings);

    /// <inheritdoc />
    public Type SettingsType => typeof(PhotosSettings);

    /// <inheritdoc />
    public Type ViewComponent => typeof(PhotosWidget);

    /// <inheritdoc />
    public Type? DataType => typeof(PhotosData);

    /// <inheritdoc />
    public WidgetSize DefaultSize => new(5, 4);

    /// <inheritdoc />
    public WidgetSize MinSize => new(3, 2);

    /// <inheritdoc />
    /// <remarks>Made-up people only: drawn placeholder pictures, never real photos.</remarks>
    public object? CreateSampleData(DisplayContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return new PhotosData(
        [
            new PhotoSlide(Guid.Parse("00000000-0000-4000-8000-0000000000a1"), Placeholder("#5E7F63", "E"), SampleCaption(context.Culture, "Erato, your daughter", "Erato, tu hija", "Η Ερατώ, η κόρη σου")),
            new PhotoSlide(Guid.Parse("00000000-0000-4000-8000-0000000000a2"), Placeholder("#2F5D7C", "N"), SampleCaption(context.Culture, "Nikos, your grandson", "Nikos, tu nieto", "Ο Νίκος, ο εγγονός σου")),
            new PhotoSlide(Guid.Parse("00000000-0000-4000-8000-0000000000a3"), Placeholder("#B7791F", "M"), SampleCaption(context.Culture, "Maria, your neighbour", "María, tu vecina", "Η Μαρία, η γειτόνισσά σου")),
        ]);
    }

    private static LocalizedText SampleCaption(CultureInfo culture, string en, string es, string el)
    {
        var text = LocalizedText.From("en", en);
        text.SetReviewed("es", es);
        text.SetReviewed("el", el);
        return text;
    }

    private static string Placeholder(string color, string initial)
    {
        var svg = $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 3'><rect width='4' height='3' fill='{color}'/><circle cx='2' cy='1.15' r='0.55' fill='rgba(255,255,255,0.85)'/><path d='M0.8 3 Q2 1.7 3.2 3Z' fill='rgba(255,255,255,0.85)'/><text x='2' y='1.35' font-size='0.6' text-anchor='middle' fill='{color}' font-family='sans-serif'>{initial}</text></svg>";
        return "data:image/svg+xml;utf8," + Uri.EscapeDataString(svg);
    }
}
