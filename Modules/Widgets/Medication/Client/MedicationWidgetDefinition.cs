using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;

namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>The medication reminders widget.</summary>
public sealed class MedicationWidgetDefinition : IWidgetDefinition
{
    /// <summary>The stable type key. Never change after release.</summary>
    public const string Key = "kinlight.medication";

    /// <inheritdoc />
    public string TypeKey => Key;

    /// <inheritdoc />
    public string NameResourceKey => nameof(MedicationStrings.WidgetName);

    /// <inheritdoc />
    public Type StringsType => typeof(MedicationStrings);

    /// <inheritdoc />
    public Type SettingsType => typeof(MedicationSettings);

    /// <inheritdoc />
    public Type ViewComponent => typeof(MedicationWidget);

    /// <inheritdoc />
    public Type? DataType => typeof(MedicationData);

    /// <inheritdoc />
    public WidgetSize DefaultSize => new(6, 2);

    /// <inheritdoc />
    public WidgetSize MinSize => new(4, 1);

    /// <inheritdoc />
    /// <remarks>Made-up medication only. A dose due right now, so the preview shows the reminder.</remarks>
    public object? CreateSampleData(DisplayContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = context.LocalNow;
        var at = new TimeOnly(now.Hour, now.Minute).AddMinutes(-5);
        var instructions = LocalizedText.From("en", "Two white tablets with a glass of water.");
        instructions.SetReviewed("es", "Dos comprimidos blancos con un vaso de agua.");
        instructions.SetReviewed("el", "Δύο άσπρα χάπια με ένα ποτήρι νερό.");

        return new MedicationData(DateOnly.FromDateTime(now),
        [
            new DoseSlot(Guid.Parse("00000000-0000-4000-8000-0000000000d1"), at, 60, instructions, TakenAt: null),
        ]);
    }
}
