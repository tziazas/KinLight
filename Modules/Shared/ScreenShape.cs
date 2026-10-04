namespace KinLight.Modules.Shared;

/// <summary>
/// The aspect ratio of the physical screen, used to frame the layout editor like the real screen.
/// </summary>
public enum ScreenShape
{
    /// <summary>16:9, most TVs and monitors.</summary>
    Wide16x9 = 0,

    /// <summary>16:10, many tablets and laptops.</summary>
    Wide16x10 = 1,

    /// <summary>4:3, older tablets and monitors.</summary>
    Standard4x3 = 2,
}
