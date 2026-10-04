using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Medication.Api;

/// <summary>Registers the medication widget's server side.</summary>
public static class MedicationApiServiceCollectionExtensions
{
    /// <summary>Adds the medication data provider. The host must also register an <see cref="IMedicationStore"/> and a <see cref="TimeProvider"/>.</summary>
    public static IServiceCollection AddKinLightMedicationWidgetProvider(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddWidgetDataProvider<MedicationDataProvider>();
    }
}
