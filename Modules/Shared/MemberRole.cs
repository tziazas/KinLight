namespace KinLight.Modules.Shared;

/// <summary>A household member's role (ARCHITECTURE.md §13). Every member has exactly one.</summary>
public enum MemberRole
{
    /// <summary>Set KinLight up; can do everything including members, devices, export and deletion.</summary>
    Owner = 0,

    /// <summary>Relatives helping out: content, layout, settings, medication schedules, confirming doses.</summary>
    Family = 1,

    /// <summary>Home aides and helpers: the Today page only.</summary>
    Caregiver = 2,
}
