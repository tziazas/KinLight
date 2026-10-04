namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>What a notification is about. The text shown on a phone is built by the host from the kind alone.</summary>
public enum NotificationKind
{
    /// <summary>A dose is due now; sent to caregivers and opted-in family.</summary>
    DoseDue = 0,

    /// <summary>A dose window closed without confirmation; sent to owners and family.</summary>
    DoseMissed = 1,
}

/// <summary>
/// A notification to one member. Deliberately carries no free text: lock-screen previews can be seen by anyone
/// nearby, so notifications never contain medication names, doses or the person's name (ARCHITECTURE.md §13).
/// </summary>
/// <param name="MemberId">The recipient.</param>
/// <param name="Kind">What it is about.</param>
/// <param name="Path">Portal path to open, e.g. "portal/today".</param>
public sealed record NotificationMessage(Guid MemberId, NotificationKind Kind, string Path);

/// <summary>Delivers notifications (Web Push with email fallback in production). Implemented by the host.</summary>
public interface INotificationSender
{
    /// <summary>Sends one notification. Failures are logged by the implementation, never thrown to callers.</summary>
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken);
}
