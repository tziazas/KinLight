using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Calendar.Client;

namespace KinLight.Modules.Widgets.Calendar.Api;

/// <summary>
/// Fetches an iCal feed and produces today's events, in the display's time zone, for the calendar widget.
/// </summary>
public sealed class CalendarFeedProvider : IWidgetDataProvider
{
    /// <summary>The named <see cref="HttpClient"/> this provider uses.</summary>
    public const string HttpClientName = "KinLight.Widgets.Calendar";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICalendarFeedStore _feedStore;
    private readonly TimeProvider _clock;

    /// <summary>Creates the provider.</summary>
    public CalendarFeedProvider(IHttpClientFactory httpClientFactory, ICalendarFeedStore feedStore, TimeProvider clock)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _feedStore = feedStore ?? throw new ArgumentNullException(nameof(feedStore));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <inheritdoc />
    public string TypeKey => CalendarWidgetDefinition.Key;

    /// <inheritdoc />
    public TimeSpan RefreshInterval => TimeSpan.FromMinutes(15);

    /// <inheritdoc />
    public async Task<object> FetchAsync(WidgetDataRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(_clock.GetUtcNow(), timeZone).DateTime);

        var feedAddress = await _feedStore.GetFeedAddressAsync(request.HouseholdId, request.PlacementId, cancellationToken).ConfigureAwait(false);
        if (feedAddress is null)
        {
            return CalendarData.Empty(today);
        }

        using var client = _httpClientFactory.CreateClient(HttpClientName);
        var ics = await client.GetStringAsync(feedAddress, cancellationToken).ConfigureAwait(false);

        return Parse(ics, today, request.TimeZoneId, request.Culture);
    }

    /// <summary>
    /// Parses iCal text into the events on <paramref name="date"/> in the given time zone.
    /// Separate from fetching so it can be tested with made-up calendars.
    /// </summary>
    public static CalendarData Parse(string ics, DateOnly date, string timeZoneId, string culture)
    {
        ArgumentNullException.ThrowIfNull(ics);
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);

        var calendar = Ical.Net.Calendar.Load(ics);
        if (calendar is null)
        {
            return CalendarData.Empty(date);
        }

        var dayStart = new CalDateTime(date.ToDateTime(TimeOnly.MinValue), timeZoneId);
        var dayEnd = new CalDateTime(date.AddDays(1).ToDateTime(TimeOnly.MinValue), timeZoneId);

        var items = new List<(DateTime SortKey, CalendarEventItem Item)>();

        foreach (var occurrence in calendar.GetOccurrences(dayStart).TakeWhileBefore(dayEnd))
        {
            if (occurrence.Source is not CalendarEvent calendarEvent)
            {
                continue;
            }

            var title = LocalizedText.From(culture, calendarEvent.Summary ?? string.Empty);
            var start = occurrence.Period.StartTime;
            var end = occurrence.Period.EffectiveEndTime;

            if (calendarEvent.IsAllDay || !start.HasTime)
            {
                items.Add((date.ToDateTime(TimeOnly.MinValue), new CalendarEventItem(title, null, null, IsAllDay: true)));
                continue;
            }

            var localStart = start.ToTimeZone(timeZoneId).Value;
            var localEnd = end?.ToTimeZone(timeZoneId).Value;

            items.Add((localStart, new CalendarEventItem(
                title,
                TimeOnly.FromDateTime(localStart),
                localEnd is null ? null : TimeOnly.FromDateTime(localEnd.Value),
                IsAllDay: false)));
        }

        return new CalendarData(date, items.OrderBy(x => x.SortKey).Select(x => x.Item).ToList());
    }
}
