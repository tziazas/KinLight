namespace KinLight.Modules.Shared;

/// <summary>Thrown when a save carries a stale version: someone else changed the display first.</summary>
public sealed class VersionConflictException : Exception
{
    /// <summary>Creates the exception.</summary>
    public VersionConflictException(long expectedVersion, long currentVersion)
        : base($"The display was changed by someone else (expected version {expectedVersion}, current {currentVersion}).")
    {
        ExpectedVersion = expectedVersion;
        CurrentVersion = currentVersion;
    }

    /// <summary>The version the caller expected.</summary>
    public long ExpectedVersion { get; }

    /// <summary>The version the display actually has.</summary>
    public long CurrentVersion { get; }
}
