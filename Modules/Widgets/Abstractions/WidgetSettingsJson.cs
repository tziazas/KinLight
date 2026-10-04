using System.Text.Json;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Serialization of widget settings. Bad or missing JSON falls back to defaults so the display never shows an error.
/// </summary>
public static class WidgetSettingsJson
{
    /// <summary>The options used for every widget's settings.</summary>
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    /// <summary>Deserializes settings, returning defaults when the JSON is null, empty or invalid.</summary>
    public static TSettings Read<TSettings>(string? json)
        where TSettings : class, new()
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new TSettings();
        }

        try
        {
            return JsonSerializer.Deserialize<TSettings>(json, Options) ?? new TSettings();
        }
        catch (JsonException)
        {
            return new TSettings();
        }
    }

    /// <summary>Deserializes settings of a runtime-known type, returning a default instance when the JSON is null, empty or invalid.</summary>
    public static object Read(string? json, Type settingsType)
    {
        ArgumentNullException.ThrowIfNull(settingsType);

        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var settings = JsonSerializer.Deserialize(json, settingsType, Options);
                if (settings is not null)
                {
                    return settings;
                }
            }
            catch (JsonException)
            {
                // Fall through to defaults: the display never shows an error.
            }
        }

        return Activator.CreateInstance(settingsType)
            ?? throw new InvalidOperationException($"Settings type {settingsType} has no parameterless constructor.");
    }

    /// <summary>Deserializes widget data of a runtime-known type, or null when the JSON is missing or invalid.</summary>
    public static object? ReadData(string? json, Type dataType)
    {
        ArgumentNullException.ThrowIfNull(dataType);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize(json, dataType, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Serializes settings.</summary>
    public static string Write<TSettings>(TSettings settings)
        where TSettings : class
    {
        ArgumentNullException.ThrowIfNull(settings);
        return JsonSerializer.Serialize(settings, Options);
    }
}
