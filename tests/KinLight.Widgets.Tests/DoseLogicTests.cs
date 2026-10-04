using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Medication.Api;
using KinLight.Modules.Widgets.Medication.Client;

namespace KinLight.Widgets.Tests;

// Made-up medications only (ARCHITECTURE.md §2, rule 7).
public class DoseLogicTests
{
    private static readonly DateOnly Monday = new(2026, 10, 5);
    private static readonly Guid MemberId = Guid.Parse("11111111-1111-4111-8111-111111111111");

    private static MedicationSchedule Daily(params TimeOnly[] times) => new()
    {
        Name = "Example tablets",
        Instructions = LocalizedText.From("en", "One tablet with water."),
        Kind = ScheduleKind.Daily,
        Doses = times.Select(t => new DoseTime(t, 60)).ToList(),
    };

    private static DoseConfirmation Confirm(MedicationSchedule s, DateOnly date, TimeOnly at, DateTimeOffset when)
        => new(Guid.NewGuid(), s.Id, date, at, when, MemberId, "Eleni");

    [Fact]
    public void Daily_schedule_has_its_doses_every_day_in_time_order()
    {
        var s = Daily(new TimeOnly(20, 0), new TimeOnly(8, 0));
        var doses = DoseLogic.OccurrencesOn(s, Monday).Select(o => o.At).ToList();
        Assert.Equal([new(8, 0), new(20, 0)], doses);
    }

    [Fact]
    public void Weekly_schedule_applies_only_on_selected_weekdays()
    {
        var s = Daily(new TimeOnly(8, 0)) with { Kind = ScheduleKind.Weekly, Weekdays = [DayOfWeek.Monday, DayOfWeek.Thursday] };
        Assert.True(DoseLogic.AppliesOn(s, Monday));
        Assert.False(DoseLogic.AppliesOn(s, Monday.AddDays(1)));
        Assert.True(DoseLogic.AppliesOn(s, Monday.AddDays(3)));
    }

    [Fact]
    public void Every_n_days_counts_from_the_anchor()
    {
        var s = Daily(new TimeOnly(8, 0)) with { Kind = ScheduleKind.EveryNDays, EveryNDays = 3, AnchorDate = Monday };
        Assert.True(DoseLogic.AppliesOn(s, Monday));
        Assert.False(DoseLogic.AppliesOn(s, Monday.AddDays(1)));
        Assert.False(DoseLogic.AppliesOn(s, Monday.AddDays(2)));
        Assert.True(DoseLogic.AppliesOn(s, Monday.AddDays(3)));
        Assert.False(DoseLogic.AppliesOn(s, Monday.AddDays(-3)), "nothing before the anchor");
    }

    [Fact]
    public void Specific_dates_and_start_end_bounds_are_honored()
    {
        var s = Daily(new TimeOnly(8, 0)) with { Kind = ScheduleKind.SpecificDates, Dates = [Monday, Monday.AddDays(10)], StartsOn = Monday, EndsOn = Monday.AddDays(7) };
        Assert.True(DoseLogic.AppliesOn(s, Monday));
        Assert.False(DoseLogic.AppliesOn(s, Monday.AddDays(10)), "after EndsOn");
        Assert.False(DoseLogic.AppliesOn(s with { IsActive = false }, Monday), "inactive");
    }

    [Fact]
    public void As_needed_never_schedules_but_logged_doses_appear_in_the_day()
    {
        var s = Daily() with { Kind = ScheduleKind.AsNeeded };
        Assert.False(DoseLogic.AppliesOn(s, Monday));

        var given = Confirm(s, Monday, new(15, 30), new DateTimeOffset(2026, 10, 5, 12, 30, 0, TimeSpan.Zero));
        var day = DoseLogic.BuildDay([s], [given], Monday, Monday.ToDateTime(new(16, 0)));

        var dose = Assert.Single(day);
        Assert.True(dose.IsAsNeeded);
        Assert.Equal(DoseStatus.Taken, dose.Status);
        Assert.Equal("Eleni", dose.Confirmation!.ConfirmedByName);
    }

    [Theory]
    [InlineData(7, 59, DoseStatus.Upcoming)]
    [InlineData(8, 0, DoseStatus.Due)]
    [InlineData(8, 59, DoseStatus.Due)]
    [InlineData(9, 0, DoseStatus.Missed)]
    public void Status_follows_the_window(int hour, int minute, DoseStatus expected)
    {
        var occurrence = new DoseOccurrence(Guid.NewGuid(), Monday, new(8, 0), 60);
        Assert.Equal(expected, DoseLogic.StatusOf(occurrence, null, Monday.ToDateTime(new(hour, minute))));
    }

    [Fact]
    public void A_confirmation_makes_the_dose_taken_even_after_the_window()
    {
        var s = Daily(new TimeOnly(8, 0));
        var late = Confirm(s, Monday, new(8, 0), new DateTimeOffset(2026, 10, 5, 7, 30, 0, TimeSpan.Zero));
        var day = DoseLogic.BuildDay([s], [late], Monday, Monday.ToDateTime(new(12, 0)));
        Assert.Equal(DoseStatus.Taken, Assert.Single(day).Status);
    }

    [Fact]
    public void Confirmations_from_another_day_do_not_count()
    {
        var s = Daily(new TimeOnly(8, 0));
        var yesterday = Confirm(s, Monday.AddDays(-1), new(8, 0), DateTimeOffset.UtcNow);
        var day = DoseLogic.BuildDay([s], [yesterday], Monday, Monday.ToDateTime(new(12, 0)));
        Assert.Equal(DoseStatus.Missed, Assert.Single(day).Status);
    }

    [Fact]
    public void Display_data_carries_instructions_and_local_taken_time_but_no_name()
    {
        var s = Daily(new TimeOnly(8, 0));
        var athens = TimeZoneInfo.FindSystemTimeZoneById("Europe/Athens");
        var confirmedUtc = new DateTimeOffset(2026, 10, 5, 5, 10, 0, TimeSpan.Zero); // 08:10 in Athens
        var data = MedicationDataProvider.Build([s], [Confirm(s, Monday, new(8, 0), confirmedUtc)], Monday, Monday.ToDateTime(new(9, 0)), athens);

        var slot = Assert.Single(data.Doses);
        Assert.Equal(new TimeOnly(8, 10), slot.TakenAt);
        Assert.Equal("One tablet with water.", slot.Instructions.Resolve(System.Globalization.CultureInfo.GetCultureInfo("en")));
        Assert.DoesNotContain(nameof(MedicationSchedule.Name), typeof(DoseSlot).GetProperties().Select(p => p.Name));
    }

    [Fact]
    public void Missed_dose_detector_reports_each_missed_dose_once()
    {
        var s = Daily(new TimeOnly(8, 0), new TimeOnly(20, 0));
        var now = Monday.ToDateTime(new(10, 0));

        var first = MissedDoseDetector.NewlyMissed([s], [], Monday, now, new HashSet<MissedDoseDetector.DoseKey>());
        var key = Assert.Single(first);
        Assert.Equal(new TimeOnly(8, 0), key.At);

        var second = MissedDoseDetector.NewlyMissed([s], [], Monday, now, new HashSet<MissedDoseDetector.DoseKey> { key });
        Assert.Empty(second);
    }

    [Theory]
    [InlineData(6, DosePart.Morning)]
    [InlineData(12, DosePart.Midday)]
    [InlineData(15, DosePart.Afternoon)]
    [InlineData(19, DosePart.Evening)]
    [InlineData(22, DosePart.Night)]
    public void Day_parts_name_doses_without_drug_names(int hour, DosePart expected)
        => Assert.Equal(expected, DoseLogic.PartOf(new TimeOnly(hour, 0)));
}
