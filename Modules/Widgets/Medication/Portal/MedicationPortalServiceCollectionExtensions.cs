using KinLight.Modules.Portal.Client;
using KinLight.Modules.Portal.Client.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Medication.Portal;

/// <summary>Registers the medication widget's portal pieces.</summary>
public static class MedicationPortalServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Medications navigation entry and the Today section. The host must register an <see cref="Client.IMedicationApi"/>
    /// and add this assembly to the router.
    /// </summary>
    public static IServiceCollection AddKinLightMedicationPortal(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddPortalNavItem<MedicationsNavItem>()
            .AddPortalTodaySection<TodayMedicationsSection>(order: 10);
    }
}
