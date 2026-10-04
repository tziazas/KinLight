namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>
/// Marker type and keys for the medication widget's <c>.resx</c> strings. One whole sentence per part of day and case
/// (ARCHITECTURE.md §15). Wording to be validated with a dementia-care professional (ARCHITECTURE.md §14).
/// </summary>
public sealed class MedicationStrings
{
    /// <summary>Medication reminders</summary>
    public const string WidgetName = nameof(WidgetName);

    /// <summary>Time for your morning medication.</summary>
    public const string TimeFor_Morning = nameof(TimeFor_Morning);

    /// <summary>Time for your midday medication.</summary>
    public const string TimeFor_Midday = nameof(TimeFor_Midday);

    /// <summary>Time for your afternoon medication.</summary>
    public const string TimeFor_Afternoon = nameof(TimeFor_Afternoon);

    /// <summary>Time for your evening medication.</summary>
    public const string TimeFor_Evening = nameof(TimeFor_Evening);

    /// <summary>Time for your night medication.</summary>
    public const string TimeFor_Night = nameof(TimeFor_Night);

    /// <summary>Morning medication: taken at {0}.</summary>
    public const string Taken_Morning = nameof(Taken_Morning);

    /// <summary>Midday medication: taken at {0}.</summary>
    public const string Taken_Midday = nameof(Taken_Midday);

    /// <summary>Afternoon medication: taken at {0}.</summary>
    public const string Taken_Afternoon = nameof(Taken_Afternoon);

    /// <summary>Evening medication: taken at {0}.</summary>
    public const string Taken_Evening = nameof(Taken_Evening);

    /// <summary>Night medication: taken at {0}.</summary>
    public const string Taken_Night = nameof(Taken_Night);

    /// <summary>Next medication at {0}.</summary>
    public const string NextDoseAt = nameof(NextDoseAt);

    /// <summary>Settings: show instructions.</summary>
    public const string Settings_ShowInstructions = nameof(Settings_ShowInstructions);

    /// <summary>Settings: show next dose.</summary>
    public const string Settings_ShowNextDose = nameof(Settings_ShowNextDose);

    /// <summary>Settings: minutes the taken sentence stays.</summary>
    public const string Settings_TakenVisibleMinutes = nameof(Settings_TakenVisibleMinutes);

    private MedicationStrings()
    {
    }
}
