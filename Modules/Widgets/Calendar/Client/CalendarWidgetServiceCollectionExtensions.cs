using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Calendar.Client;

/// <summary>Registers the calendar widget's browser side.</summary>
public static class CalendarWidgetServiceCollectionExtensions
{
    /// <summary>Adds the calendar widget.</summary>
    public static IServiceCollection AddKinLightCalendarWidget(this IServiceCollection services)
        => services.AddWidget<CalendarWidgetDefinition>();
}
