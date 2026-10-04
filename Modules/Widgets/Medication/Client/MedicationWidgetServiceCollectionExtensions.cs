using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Medication.Client;

/// <summary>Registers the medication widget's browser side.</summary>
public static class MedicationWidgetServiceCollectionExtensions
{
    /// <summary>Adds the medication reminders widget.</summary>
    public static IServiceCollection AddKinLightMedicationWidget(this IServiceCollection services)
        => services.AddWidget<MedicationWidgetDefinition>();
}
