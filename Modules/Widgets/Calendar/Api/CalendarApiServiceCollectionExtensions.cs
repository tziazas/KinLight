using KinLight.Modules.Widgets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinLight.Modules.Widgets.Calendar.Api;

/// <summary>Registers the calendar widget's server side.</summary>
public static class CalendarApiServiceCollectionExtensions
{
    /// <summary>
    /// Adds the calendar feed provider. The host must also register an <see cref="ICalendarFeedStore"/>
    /// and a <see cref="TimeProvider"/>.
    /// </summary>
    public static IServiceCollection AddKinLightCalendarWidgetProvider(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpClient(CalendarFeedProvider.HttpClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("KinLight/1.0 (+https://github.com/tziazas/KinLight)");
        });

        return services.AddWidgetDataProvider<CalendarFeedProvider>();
    }
}
