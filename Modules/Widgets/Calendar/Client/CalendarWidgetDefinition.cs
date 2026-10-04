using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;

namespace KinLight.Modules.Widgets.Calendar.Client;

/// <summary>The calendar widget.</summary>
public sealed class CalendarWidgetDefinition : IWidgetDefinition
{
    /// <summary>The stable type key. Never change after release.</summary>
    public const string Key = "kinlight.calendar";

    /// <inheritdoc />
    public string TypeKey => Key;

    /// <inheritdoc />
    public string NameResourceKey => nameof(CalendarStrings.WidgetName);

    /// <inheritdoc />
    public Type StringsType => typeof(CalendarStrings);

    /// <inheritdoc />
    public Type SettingsType => typeof(CalendarSettings);

    /// <inheritdoc />
    public Type ViewComponent => typeof(CalendarWidget);

    /// <inheritdoc />
    public Type? DataType => typeof(CalendarData);

    /// <inheritdoc />
    public WidgetSize DefaultSize => new(6, 3);

    /// <inheritdoc />
    public WidgetSize MinSize => new(4, 2);

    /// <inheritdoc />
    /// <remarks>Made-up people and events only.</remarks>
    public object? CreateSampleData(DisplayContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var culture = context.Culture.Name;
        var now = context.LocalNow;
        var date = DateOnly.FromDateTime(now);

        // Times are relative to now so the preview always shows something upcoming, but never cross midnight.
        var endOfDay = now.Date.AddDays(1).AddMinutes(-1);
        TimeOnly In(int minutes)
        {
            var at = now.AddMinutes(minutes);
            return TimeOnly.FromDateTime(at > endOfDay ? endOfDay : at);
        }

        return new CalendarData(date,
        [
            new CalendarEventItem(LocalizedText.From(culture, "Nikos is visiting"), In(45), In(105), IsAllDay: false),
            new CalendarEventItem(LocalizedText.From(culture, "Afternoon walk with Maria"), In(180), In(240), IsAllDay: false),
            new CalendarEventItem(LocalizedText.From(culture, "Call from Eleni"), In(300), In(320), IsAllDay: false),
        ]);
    }
}
