namespace KinLight.Modules.Widgets.Calendar.Api;

/// <summary>
/// Gives the provider the secret iCal address for a placement. Implemented by the host, which stores
/// addresses encrypted at rest (ARCHITECTURE.md §2, rule 3). Lookups are always scoped to a household.
/// </summary>
public interface ICalendarFeedStore
{
    /// <summary>Returns the feed address for a placement, or null if none is configured.</summary>
    Task<Uri?> GetFeedAddressAsync(Guid householdId, Guid placementId, CancellationToken cancellationToken);
}
