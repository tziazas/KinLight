using System.Globalization;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Abstractions;
using MudBlazor;

namespace KinLight.Modules.Widgets.Medication.Portal;

/// <summary>The "Medications" entry: Owner and Family only (ARCHITECTURE.md §13).</summary>
public sealed class MedicationsNavItem : IPortalNavItem
{
    /// <inheritdoc />
    public string Href => "portal/medications";

    /// <inheritdoc />
    public string Icon => Icons.Material.Filled.Medication;

    /// <inheritdoc />
    public int Order => 20;

    /// <inheritdoc />
    public string Label => WidgetStrings.For<MedicationPortalStrings>(CultureInfo.CurrentUICulture)[MedicationPortalStrings.Nav_Medications];

    /// <inheritdoc />
    public bool IsVisibleTo(MemberRole role) => role != MemberRole.Caregiver;
}
