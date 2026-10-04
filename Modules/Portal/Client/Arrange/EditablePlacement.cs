using KinLight.Modules.Shared;

namespace KinLight.Modules.Portal.Client.Arrange;

/// <summary>A placement being edited. Mutable copy of <see cref="WidgetPlacement"/> owned by the editor's model.</summary>
public sealed class EditablePlacement
{
    /// <summary>Creates an editable copy.</summary>
    public EditablePlacement(WidgetPlacement source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Id = source.Id;
        TypeKey = source.TypeKey;
        X = source.X;
        Y = source.Y;
        Width = source.Width;
        Height = source.Height;
        FontScale = source.FontScale;
        HighContrast = source.HighContrast;
        IsEnabled = source.IsEnabled;
        VisibleFrom = source.VisibleFrom;
        VisibleTo = source.VisibleTo;
        SettingsJson = source.SettingsJson;
    }

    /// <summary>The placement id.</summary>
    public Guid Id { get; }

    /// <summary>The widget type key.</summary>
    public string TypeKey { get; }

    /// <summary>Column.</summary>
    public int X { get; set; }

    /// <summary>Row.</summary>
    public int Y { get; set; }

    /// <summary>Width in columns.</summary>
    public int Width { get; set; }

    /// <summary>Height in rows.</summary>
    public int Height { get; set; }

    /// <summary>Text size multiplier.</summary>
    public double FontScale { get; set; }

    /// <summary>High contrast.</summary>
    public bool HighContrast { get; set; }

    /// <summary>Shown at all.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Daily visibility start.</summary>
    public TimeOnly? VisibleFrom { get; set; }

    /// <summary>Daily visibility end.</summary>
    public TimeOnly? VisibleTo { get; set; }

    /// <summary>Widget settings JSON.</summary>
    public string? SettingsJson { get; set; }

    /// <summary>Converts back to the wire record.</summary>
    public WidgetPlacement ToPlacement() => new(Id, TypeKey, X, Y, Width, Height, FontScale, HighContrast, IsEnabled, VisibleFrom, VisibleTo, SettingsJson);
}
