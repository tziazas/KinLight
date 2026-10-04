using System.Globalization;
using KinLight.Modules.Widgets.Abstractions;
using KinLight.Modules.Widgets.Medication.Client;

namespace KinLight.Modules.Widgets.Medication.Portal;

/// <summary>Portal-side wording helpers in the portal's language.</summary>
internal static class MedicationText
{
    public static IWidgetStrings T => WidgetStrings.For<MedicationPortalStrings>(CultureInfo.CurrentUICulture);

    public static string KindLabel(ScheduleKind kind) => kind switch
    {
        ScheduleKind.Daily => T[MedicationPortalStrings.Kind_Daily],
        ScheduleKind.Weekly => T[MedicationPortalStrings.Kind_Weekly],
        ScheduleKind.EveryNDays => T[MedicationPortalStrings.Kind_EveryNDays],
        ScheduleKind.SpecificDates => T[MedicationPortalStrings.Kind_SpecificDates],
        _ => T[MedicationPortalStrings.Kind_AsNeeded],
    };

    public static string StatusLabel(DoseStatus status) => status switch
    {
        DoseStatus.Upcoming => T[MedicationPortalStrings.Status_Upcoming],
        DoseStatus.Due => T[MedicationPortalStrings.Status_Due],
        DoseStatus.Taken => T[MedicationPortalStrings.Status_Taken],
        _ => T[MedicationPortalStrings.Status_Missed],
    };

    public static string Summary(MedicationSchedule s)
    {
        var culture = CultureInfo.CurrentCulture;
        var times = string.Join(", ", s.Doses.OrderBy(d => d.At).Select(d => d.At.ToString("t", culture)));
        return s.Kind switch
        {
            ScheduleKind.Daily => T[MedicationPortalStrings.Summary_Daily, times],
            ScheduleKind.Weekly => T[MedicationPortalStrings.Summary_Weekly, times, string.Join(", ", s.Weekdays.OrderBy(d => ((int)d + 6) % 7).Select(d => culture.DateTimeFormat.GetAbbreviatedDayName(d)))],
            ScheduleKind.EveryNDays => T[MedicationPortalStrings.Summary_EveryNDays, times, s.EveryNDays],
            ScheduleKind.SpecificDates => T[MedicationPortalStrings.Summary_SpecificDates, times, s.Dates.Count],
            _ => T[MedicationPortalStrings.Summary_AsNeeded],
        };
    }

    public static string Time(TimeOnly t) => t.ToString("t", CultureInfo.CurrentCulture);
}
