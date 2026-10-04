using KinLight.Modules.Shared;
using Microsoft.AspNetCore.Components;

namespace KinLight.Modules.Widgets.Abstractions;

/// <summary>
/// Base class for a widget view with settings and server-fetched data (ARCHITECTURE.md §14).
/// The host passes the placement, the deserialized settings and the last good data. A widget must render
/// something sensible when <see cref="Data"/> is null, and must never show an error.
/// </summary>
/// <typeparam name="TSettings">The widget's settings class.</typeparam>
/// <typeparam name="TData">The widget's data type.</typeparam>
public abstract class WidgetComponentBase<TSettings, TData> : WidgetComponentBase<TSettings>
    where TSettings : class, new()
    where TData : class
{
    /// <summary>The last good data fetched by the server, or null if none has arrived yet.</summary>
    [Parameter]
    public TData? Data { get; set; }
}

/// <summary>
/// Base class for a widget view with settings and no outside data.
/// </summary>
/// <typeparam name="TSettings">The widget's settings class.</typeparam>
public abstract class WidgetComponentBase<TSettings> : ComponentBase
    where TSettings : class, new()
{
    /// <summary>Where and how the widget is placed on the grid.</summary>
    [Parameter, EditorRequired]
    public required WidgetPlacement Placement { get; set; }

    /// <summary>The widget's settings. Defaults are used when the stored JSON is missing or invalid.</summary>
    [Parameter]
    public TSettings Settings { get; set; } = new();

    /// <summary>The display this widget is rendered on.</summary>
    [CascadingParameter]
    public required DisplayContext Context { get; set; }

    /// <summary>Built-in text of <typeparamref name="TStrings"/> in the display's language.</summary>
    protected IWidgetStrings Strings<TStrings>() => WidgetStrings.For<TStrings>(Context.Culture);
}
