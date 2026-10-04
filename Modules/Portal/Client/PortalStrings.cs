namespace KinLight.Modules.Portal.Client;

/// <summary>
/// Marker type and keys for the portal's <c>.resx</c> strings (ARCHITECTURE.md §15). The portal's language follows the
/// signed-in person (the browser for now), independent of any display's language. Whole sentences per case; plural
/// forms are separate keys because resx has no plural rules.
/// </summary>
public sealed class PortalStrings
{
    /// <summary>KinLight</summary>
    public const string AppName = nameof(AppName);

    /// <summary>Today</summary>
    public const string Nav_Today = nameof(Nav_Today);

    /// <summary>Displays</summary>
    public const string Nav_Displays = nameof(Nav_Displays);

    /// <summary>Displays</summary>
    public const string Displays_Title = nameof(Displays_Title);

    /// <summary>No displays yet. Pair a screen from the Devices page to get started.</summary>
    public const string Displays_Empty = nameof(Displays_Empty);

    /// <summary>Arrange</summary>
    public const string Action_Arrange = nameof(Action_Arrange);

    /// <summary>Settings</summary>
    public const string Action_Settings = nameof(Action_Settings);

    /// <summary>Save</summary>
    public const string Action_Save = nameof(Action_Save);

    /// <summary>Cancel</summary>
    public const string Action_Cancel = nameof(Action_Cancel);

    /// <summary>1 widget</summary>
    public const string WidgetCount_One = nameof(WidgetCount_One);

    /// <summary>{0} widgets</summary>
    public const string WidgetCount_Other = nameof(WidgetCount_Other);

    /// <summary>16:9</summary>
    public const string Shape_16x9 = nameof(Shape_16x9);

    /// <summary>16:10</summary>
    public const string Shape_16x10 = nameof(Shape_16x10);

    /// <summary>4:3</summary>
    public const string Shape_4x3 = nameof(Shape_4x3);

    /// <summary>Settings</summary>
    public const string Settings_Title = nameof(Settings_Title);

    /// <summary>Name</summary>
    public const string Settings_Name = nameof(Settings_Name);

    /// <summary>Give the display a name.</summary>
    public const string Settings_NameRequired = nameof(Settings_NameRequired);

    /// <summary>Screen shape</summary>
    public const string Settings_Shape = nameof(Settings_Shape);

    /// <summary>The layout editor is framed like the real screen.</summary>
    public const string Settings_ShapeHelp = nameof(Settings_ShapeHelp);

    /// <summary>16:9 (most TVs and monitors)</summary>
    public const string Settings_Shape16x9 = nameof(Settings_Shape16x9);

    /// <summary>16:10 (many tablets)</summary>
    public const string Settings_Shape16x10 = nameof(Settings_Shape16x10);

    /// <summary>4:3 (older tablets)</summary>
    public const string Settings_Shape4x3 = nameof(Settings_Shape4x3);

    /// <summary>Display language</summary>
    public const string Settings_Language = nameof(Settings_Language);

    /// <summary>The person's first language. Independent of the device and of this portal.</summary>
    public const string Settings_LanguageHelp = nameof(Settings_LanguageHelp);

    /// <summary>Backup language</summary>
    public const string Settings_BackupLanguage = nameof(Settings_BackupLanguage);

    /// <summary>Used when text is missing in the display language.</summary>
    public const string Settings_BackupLanguageHelp = nameof(Settings_BackupLanguageHelp);

    /// <summary>Time zone</summary>
    public const string Settings_TimeZone = nameof(Settings_TimeZone);

    /// <summary>Choose a time zone.</summary>
    public const string Settings_TimeZoneRequired = nameof(Settings_TimeZoneRequired);

    /// <summary>Night hours</summary>
    public const string Settings_NightHours = nameof(Settings_NightHours);

    /// <summary>Between these hours the screen dims and says it is night-time. Set both to the same time to turn this off.</summary>
    public const string Settings_NightHoursHelp = nameof(Settings_NightHoursHelp);

    /// <summary>Night begins</summary>
    public const string Settings_NightBegins = nameof(Settings_NightBegins);

    /// <summary>Night ends</summary>
    public const string Settings_NightEnds = nameof(Settings_NightEnds);

    /// <summary>Saved. The screen will update in a moment.</summary>
    public const string Saved = nameof(Saved);

    /// <summary>Someone else changed this display first. Reload to see their changes before saving.</summary>
    public const string Conflict = nameof(Conflict);

    /// <summary>Arrange</summary>
    public const string Arrange_Title = nameof(Arrange_Title);

    /// <summary>Unsaved changes</summary>
    public const string Arrange_Unsaved = nameof(Arrange_Unsaved);

    /// <summary>Drag to move. Drag the corner to resize. Select a widget to change its text size.</summary>
    public const string Arrange_Hint = nameof(Arrange_Hint);

    /// <summary>Text size: {0}×</summary>
    public const string Arrange_TextSize = nameof(Arrange_TextSize);

    /// <summary>High contrast</summary>
    public const string Arrange_HighContrast = nameof(Arrange_HighContrast);

    /// <summary>Shown on the screen</summary>
    public const string Arrange_Shown = nameof(Arrange_Shown);

    /// <summary>Remove from screen</summary>
    public const string Arrange_Remove = nameof(Arrange_Remove);

    /// <summary>Add a widget</summary>
    public const string Arrange_AddWidget = nameof(Arrange_AddWidget);

    /// <summary>You have unsaved changes. Navigate again to leave without saving.</summary>
    public const string Arrange_LeaveWarning = nameof(Arrange_LeaveWarning);

    /// <summary>Widget settings</summary>
    public const string Arrange_WidgetSettings = nameof(Arrange_WidgetSettings);

    /// <summary>OK</summary>
    public const string Dialog_Ok = nameof(Dialog_Ok);

    /// <summary>This widget has no settings.</summary>
    public const string Widget_NoSettings = nameof(Widget_NoSettings);

    /// <summary>Type a city in English, for example Athens or London.</summary>
    public const string Settings_TimeZoneHelp = nameof(Settings_TimeZoneHelp);

    private PortalStrings()
    {
    }
}
