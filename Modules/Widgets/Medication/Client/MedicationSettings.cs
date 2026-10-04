using System.ComponentModel.DataAnnotations;

namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>Settings for the medication widget. Labels are keys in <see cref="MedicationStrings"/>.</summary>
public sealed class MedicationSettings
{
    /// <summary>Show the caregiver-written instructions under the reminder.</summary>
    [Display(Name = nameof(MedicationStrings.Settings_ShowInstructions))]
    public bool ShowInstructions { get; set; } = true;

    /// <summary>Between doses, quietly show when the next one is.</summary>
    [Display(Name = nameof(MedicationStrings.Settings_ShowNextDose))]
    public bool ShowNextDose { get; set; } = true;

    /// <summary>How long the "taken at" sentence stays after a confirmation.</summary>
    [Display(Name = nameof(MedicationStrings.Settings_TakenVisibleMinutes))]
    [Range(5, 720)]
    public int TakenVisibleMinutes { get; set; } = 120;
}
