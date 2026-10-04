using KinLight.Modules.Widgets.BuiltIn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;
using MudBlazor.Translations;

namespace KinLight.Modules.Portal.Client;

/// <summary>
/// Registers the services the portal's Blazor WebAssembly components need.
/// </summary>
public static class PortalClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds the KinLight portal client services, including MudBlazor and the built-in widgets, to <paramref name="services"/>.
    /// </summary>
    public static IServiceCollection AddKinLightPortalClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Widget names, default sizes and settings types for the Arrange and Widget settings pages (ARCHITECTURE.md §13).
        services.AddLocalization();
        services.AddKinLightBuiltInWidgets();
        services.TryAddSingleton(TimeProvider.System);

        // MudBlazor's own strings (pickers, tables) in the portal's language.
        services.AddMudTranslations();

        services.AddMudServices(options =>
        {
            // Calm and plain: short-lived, bottom-centre snackbars that never pile up.
            options.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomCenter;
            options.SnackbarConfiguration.PreventDuplicates = true;
            options.SnackbarConfiguration.MaxDisplayedSnackbars = 2;
            options.SnackbarConfiguration.VisibleStateDuration = 4000;
        });

        // The host must register an IPortalApi (HTTP in production, a fake in development hosts).
        return services;
    }
}
