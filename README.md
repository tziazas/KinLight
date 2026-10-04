# KinLight

**A calm, always-on screen for someone living with dementia, kept up to date by their family from their phones.**

Watching someone you love lose track of the day, the time, and even the faces around them is one of the hardest parts of Alzheimer's and other dementias. Small, steady reminders help: what day it is, whether it is morning or evening, who is in a photo and how they are related, what is happening today.

KinLight turns a Raspberry Pi with a screen, an old tablet, or a TV into that reminder. Family members and caregivers manage what it shows from a phone-friendly portal. It is open source and free to self-host, with an optional low-cost hosted version planned at kinlight.care for families who do not want to set up hardware.

> **Status: early development.** The display, the portal, the layout editor and the first three widgets work end to end against an in-browser development store. There is no server, no accounts and no pairing yet. See [What works today](#what-works-today) before planning a deployment. The full design is in [ARCHITECTURE.md](ARCHITECTURE.md).

---

## Contents

- [The two faces of KinLight](#the-two-faces-of-kinlight)
- [Capabilities](#capabilities)
- [What works today](#what-works-today)
- [Design principles for the display](#design-principles-for-the-display)
- [Privacy and responsibility](#privacy-and-responsibility)
- [Editions and self-hosting](#editions-and-self-hosting)
- [Running the development host](#running-the-development-host)
- [Repository layout](#repository-layout)
- [Creating a widget](#creating-a-widget)
- [Adding a language](#adding-a-language)
- [Contributing](#contributing)
- [License](#license)

---

## The two faces of KinLight

**The display** is what the person living with dementia sees: a full-screen board of widgets that needs no touching, no logging in, and never shows an error. If the internet or the server goes away, it keeps showing what it last knew. At night it dims and says it is night-time.

**The portal** is what family members and caregivers use, mostly on a phone. They arrange widgets on a picture of the actual screen, make text bigger or smaller per widget, choose the display's language and time zone, set night hours, add photos and messages, pair new screens and invite others. Caregivers such as home aides see a simpler Today page where they confirm medication they have given. Saved changes reach the screen within seconds.

---

## Capabilities

This is the complete intended feature set. Items are marked **done**, **partly done**, or **planned**. Everything planned is specified in [ARCHITECTURE.md](ARCHITECTURE.md).

### Widgets on the display

| Widget | Status | What it does |
|---|---|---|
| Clock and day | **Done** | A plain sentence such as "It is Saturday afternoon.", the time, and the date written out in full. At night the sentence changes and the weekday is dropped on purpose. |
| Calendar | **Partly done** | Shows what is next today, in plain language, from a Google Calendar secret iCal address (no OAuth). The view and the server-side feed parser exist; the encrypted address storage and the server are not built yet. |
| Photos | **Done** | Rotates family photos slowly with a caption saying who the person is to the viewer, for example "Erato, your daughter". Captions are entered per language. Photos are grouped into albums; a screen can show one album or all photos. Caption over or under the photo, interval, order and visibility are per-widget settings. |
| Medication reminders | **Partly done** | A large reminder during each dose window ("Time for your morning medication." plus the caregiver-written instructions), with no button on the screen. The caregiver who gives the dose confirms it with one tap on the portal's Today page; the log records who and when. Once confirmed the screen says "Morning medication: taken at 8:10." Schedules can be daily, on chosen weekdays, every N days, on specific dates, or as needed. Missed doses are flagged in the portal; the phone notifications that prompt caregivers and alert family need the server. Documented as a supplement, never the safety mechanism. |
| Family messages | Planned | Short messages from family, such as "Nikos is visiting at 3." |
| Weather | Later | Plain language: "Cold today, wear a coat." |

Widgets are a plug-in system. See [Creating a widget](#creating-a-widget).

### Display behavior

| Capability | Status |
|---|---|
| Fixed 12 × 6 grid, per-widget text size and high contrast | **Done** |
| Per-widget visibility hours (show a widget only between certain times) | **Partly done**: the display honors them; the portal has no control for them yet |
| Night mode: dims the screen and says it is night-time between configured hours | **Done** |
| Display language independent of the device and of the portal | **Done** |
| Never shows an error, a spinner that does not resolve, or a connection message | **Done** |
| Reduced-motion support | **Done** |
| Last-good data kept when a refresh fails | **Done** |
| Offline: installable PWA with cached app and data, keeps working without the server | Planned |
| Real-time updates: the server pings the display, it refetches | Planned (interface exists) |
| Pairing a screen with a six-digit code, like a streaming app on a TV | Planned |

### Portal

| Page | Status | Who | What it does |
|---|---|---|---|
| Displays | **Done** | Owner, Family | The household's screens, each with Arrange and Settings. |
| Arrange | **Done** | Owner, Family | The layout editor, framed as the actual screen in its real shape (16:9, 16:10, 4:3). Drag to move, drag the corner to resize, select to change text size, contrast and visibility. Live previews with made-up sample data. Nothing reaches the screen until Save; leaving with unsaved changes warns first. Saves are guarded against two people overwriting each other. |
| Widget settings | **Done** | Owner, Family | A form generated from each widget's settings class, opened from the Arrange page. |
| Settings | **Done** | Owner, Family | Name, screen shape, display language, backup language, time zone, night hours. |
| Photos | **Done** | Owner, Family | Upload (downscaled in the browser before upload), caption per language, albums, delete. |
| Today | **Partly done** | All, landing page for Caregivers | Today's medications with a large Confirm button per dose, and "given now" for as-needed medication. Built as an extension point, so what is happening today and the short-message box can join it. |
| Medications | **Done** | Owner, Family | Schedules, dose windows, confirmation history for the last seven days (who, when, missed). |
| Devices | Planned | Owner | Pair a new screen, rename, revoke a lost tablet. |
| Members | Planned | Owner | Invite by link or email, set roles and access expiry, remove access. |
| Export and delete | Planned | Owner | Download everything the household has; delete the household. |
| Notifications | Planned | All | Turn phone notifications on or off for this device. |

### Accounts, households and safety

| Capability | Status |
|---|---|
| Households as the unit of isolation; one household never sees another's data, enforced in three independent layers and covered by tests that are never skipped | Planned |
| Three roles: Owner, Family, Caregiver, enforced on every command, not just by hiding navigation | **Partly done**: the portal knows the signed-in member's role and hides and redirects accordingly; server-side enforcement comes with the server |
| Accounts always per person, never shared; caregivers join by invitation link and sign in with a passkey | Planned |
| Access expiry for caregivers, for example the end of an agency assignment | Planned |
| Secrets (calendar addresses, device tokens) encrypted at rest or stored only as hashes | Planned |
| Phone notifications by Web Push, no app store app, with email fallback. Notification text never contains medication names, doses or the person's name | Planned (the notification contract and missed-dose detection exist; delivery needs the server) |
| One-click export of everything and full deletion, in both editions | Planned |
| No analytics, trackers, advertising or third-party scripts, ever | **Done** (by rule) |

### Languages

| Capability | Status |
|---|---|
| Built-in text as whole sentences per language, never assembled from fragments | **Done** |
| Display languages: English, Spanish, Greek | **Done** |
| Portal languages: English, Greek | **Done** |
| Text families enter (captions, messages) stored per language with fallback rules | **Done** |
| A test that fails when any offered language is incompletely translated | **Done** |
| Optional machine translation that fills gaps and marks them unreviewed; safety-relevant text never shows an unreviewed translation | Planned |
| Right-to-left languages | Later (CSS already uses logical properties) |

---

## What works today

You can clone the repository, build it, and run a single development host that serves the display and the portal from one Blazor WebAssembly app with an in-browser fake store (localStorage and IndexedDB). In that host you can:

- see the display with the clock, calendar sample data and photos;
- open the portal, arrange widgets on a framed screen, change settings and save;
- upload photos, caption them per language, group them into albums, and watch the display pick them up;
- switch the display language between English, Spanish and Greek and see the whole screen follow, including fonts and date formats.

You cannot yet run KinLight for a real household. There is no server, database, sign-in, pairing or real-time channel. The `KinLight.Server` executable, Docker image and Raspberry Pi instructions described below are the plan, not the present.

---

## Design principles for the display

These come from how dementia affects perception and memory. They override visual preferences and are binding for every contributor. The full list with implementation notes is in [ARCHITECTURE.md §3](ARCHITECTURE.md#3-design-principles-for-the-display).

- **Never show an error.** Widgets render inside an error boundary with empty error content. A failed refresh keeps the last good data.
- **Plain language.** "It is Saturday afternoon." rather than "Sat 14:32". Dates are written out.
- **Whole-sentence translations.** One resource string per case. Word order and grammar differ between languages.
- **No interaction required.** Nothing on the display offers or requires a touch. Many screens are TVs with no touchscreen. Anything that needs confirming is confirmed by a caregiver in the portal.
- **Stable and calm.** Fixed layout, minimal and slow animation, respects reduced-motion settings.
- **Upcoming, not past.** Calendars show what is next, not what already happened.
- **Relationships, not just names.** Photo captions say who the person is to the viewer.
- **Night mode.** A glance at 3 a.m. should not prompt getting dressed.
- **The person's first language.** Bilingual people often lose their later-learned language first.
- **Legibility.** Atkinson Hyperlegible Next, designed by the Braille Institute for low-vision readers, is bundled. Greek is covered by bundled Noto Sans subsets. Text size and contrast are adjustable per widget.

---

## Privacy and responsibility

KinLight holds some of the most personal information a family has, about a person who may not be able to understand or consent to how it is used: photos with names and relationships, calendars that show when the person is home alone, medication schedules, and secret calendar addresses. The rules that follow are in [ARCHITECTURE.md §2](ARCHITECTURE.md#2-sensitivity-and-responsibility). Two of them matter to anyone touching the code:

- **Test with made-up people.** Never use real family photos, names or medications in development, tests, bug reports, screenshots, or AI coding tools. The sample data in this repository is invented.
- **Medication reminders are a supplement, never the safety mechanism.** The person living with dementia is never asked to confirm a dose. The caregiver who gives it confirms it.

---

## Editions and self-hosting

KinLight runs the same code in two editions. Everything that protects a family's data lives in this public repository, where anyone can inspect it. The hosted edition adds billing and operations in a private repository that only consumes the public packages.

| | Self-hosted (free, open source) | Hosted at kinlight.care (paid, planned) |
|---|---|---|
| Who it is for | Families comfortable setting up a Raspberry Pi or small server | Families who want it to just work |
| Runs on | Raspberry Pi (Docker), any Linux, Windows or macOS machine | Cloud infrastructure |
| Database | SQLite in a data folder | PostgreSQL |
| Photos and files | Local disk | Azure Blob Storage |
| Households | One by default, more allowed | Many |
| Updates and backups | The family's responsibility; backing up is copying one folder | Included |
| Remote access | Tailscale recommended, never port forwarding | Built in, HTTPS |

**Export promise:** hosted families will be able to export everything at any time and move to a self-hosted install. If the hosted service ever shuts down, that path remains.

### Self-hosting on a Raspberry Pi (planned)

The target experience, once `KinLight.Server` exists:

1. Install 64-bit Raspberry Pi OS and Docker.
2. Clone this repository and run:
   ```sh
   docker compose up -d --build
   ```
   This uses the standard `mcr.microsoft.com/dotnet/aspnet:10.0` image, which includes ICU so dates and day names appear in the display's language rather than silently falling back to English. Data lives in a named volume: the SQLite database, uploaded photos and the keys that keep everyone signed in across restarts.
3. Point Chromium in kiosk mode at `http://localhost:8080/` from the labwc autostart file. The screen shows a six-digit pairing code.
4. On your phone, open the portal, enter the code, and choose which display this screen is.
5. For access to the portal from outside the house, install Tailscale on the Pi. It also provides the HTTPS certificate the portal needs on phones. Do not forward the port to the internet.

A command-line flag will reset the owner password for installs without email configured.

---

## Running the development host

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (the repository pins 10.0.x in `global.json`)
- [Node.js](https://nodejs.org/) 20 or later. Only the display's Tailwind CSS build needs it; MSBuild runs `npm ci` for you on first build.
- A modern browser. On a Mac the browser-based checks in this repository use the installed Chrome.

### Build and run

```sh
git clone https://github.com/tziazas/KinLight.git
cd KinLight
dotnet build KinLight.slnx
cd KinLight/KinLight.Client
dotnet run
```

Then open:

- **Display:** http://localhost:5180/
- **Portal:** http://localhost:5180/portal

The host seeds one display called "Kitchen" with a clock and one made-up medication schedule. A switcher in the portal's app bar picks which made-up household member is "signed in" (an Owner, a Family member or a Caregiver), so role-dependent pages and the confirmation log can be exercised. Everything you change is kept in the browser's localStorage and IndexedDB under keys starting with `kinlight.dev.`; clear them to start over. There is no button on the display that leads to the portal, by design: the display never offers a control.

The dev host is a harness, not the production shape. In production the display is its own installable app with a fixed language and an offline service worker, and the portal is a separate app in the signed-in person's language. In the dev host both share one process, so the portal runs in the display's language.

### Tests

```sh
dotnet test KinLight.slnx
```

The suite checks translation completeness (every translated resource file has exactly the keys of its English source, no value is empty, and the languages offered to families are exactly the ones that are fully translated) and the medication dose rules (schedule kinds, windows, status transitions, confirmations, missed-dose detection).

---

## Repository layout

The code is organized as modules, each with a browser-safe `Client` project and, where there is server-side work, an `Api` project. References only point inward and nothing that runs in the browser ever references server code.

```
KinLight.slnx
ARCHITECTURE.md                          The design. Read it before changing anything the display shows.
Directory.Build.props                    Shared build settings; warnings are errors.
Directory.Packages.props                 Central package versions.

Modules/
  Shared/                                Wire contracts: DisplayConfig, WidgetPlacement, LocalizedText,
                                         NightMode, BoardGrid, KnownCultures. No dependencies.
  Display/
    Client/                              The screen: board, widget host, display page, boot script. Blazor
                                         WebAssembly library. Tailwind CSS, Atkinson Hyperlegible Next bundled.
    Api/                                 Display minimal API endpoints (route group; endpoints are added as the
                                         server is built).
  Portal/
    Client/                              The family and caregiver app: pages, layout editor (gridstack.js),
                                         generated widget settings form, LocalizedText editor, extension points.
                                         Blazor WebAssembly library with MudBlazor.
    Api/                                 Portal minimal API endpoints (route group).
  Widgets/
    Abstractions/                        IWidgetDefinition, WidgetComponentBase, DisplayContext, IWidgetDataProvider,
                                         WidgetStrings, WidgetView, registration helpers.
    BuiltIn/                             The single list of built-in widgets; display and portal both call it.
    Clock/Client/                        Clock and day widget (no server side).
    Calendar/Client/  Calendar/Api/      Calendar widget and its iCal feed provider (Ical.Net).
    Photos/Client/  Photos/Api/  Photos/Portal/
                                         Photos widget, its data provider and image endpoint, and the portal's
                                         photo library page.
    Medication/Client/  Medication/Api/  Medication/Portal/
                                         Medication reminders: scheduling model and dose logic, data provider and
                                         missed-dose detection, Medications page and Today section.

KinLight/
  KinLight.Client/                       Development host: display at /, portal at /portal, in-browser fakes.

tests/
  KinLight.Localization.Tests/           Translation completeness.
  KinLight.Widgets.Tests/                Dose scheduling and status rules.
```

ARCHITECTURE.md §8 describes a `src/` layout with slightly different project names. The Modules layout above is the one in use; the document is being updated to match.

---

## Creating a widget

A widget is three things: a **definition** that describes it, a **view** that renders it on the display, and optionally a **data provider** on the server that fetches outside data on a schedule. Each widget is its own folder under `Modules/Widgets/` with one project per side. This section walks through a complete example, a "Family messages" widget, and then lists the rules.

### 1. Decide what the widget needs

Ask three questions:

- **Does it need data from outside the browser?** The clock does not. A calendar or weather widget does. If yes, you need an `Api` project with a data provider, and a data record that travels to the display.
- **Does it need a management page in the portal?** Photos needs a library page. Most widgets only need settings, which the portal generates for you. If yes, you need a `Portal` project.
- **What must the family be able to configure?** That becomes the settings class.

Pick a **type key**, lowercase with a dot, such as `kinlight.messages`. It is stored in the database with every placement and can never change after release.

### 2. Create the Client project

```
Modules/Widgets/Messages/Client/KinLight.Modules.Widgets.Messages.Client.csproj
```

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <Description>KinLight family messages widget.</Description>
  </PropertyGroup>
  <ItemGroup>
    <SupportedPlatform Include="browser" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="../../Abstractions/KinLight.Modules.Widgets.Abstractions.csproj" />
  </ItemGroup>
</Project>
```

The Client project runs in the browser on the display and in the portal. It may reference only `Widgets.Abstractions` and, through it, `Shared`. It must never reference MudBlazor, Entity Framework, ASP.NET Core server packages, another widget, the Display or the Portal. The build will not stop you, so this is a review rule.

Add an `_Imports.razor`:

```razor
@using Microsoft.AspNetCore.Components.Web
@using KinLight.Modules.Shared
@using KinLight.Modules.Widgets.Abstractions
@using KinLight.Modules.Widgets.Messages.Client
```

### 3. Strings: a marker class and resource files

Every piece of text the widget shows or the portal needs for it lives in `.resx` files, one whole sentence per case. Create a marker class whose constants name the keys:

```csharp
namespace KinLight.Modules.Widgets.Messages.Client;

public sealed class MessagesStrings
{
    public const string WidgetName = nameof(WidgetName);          // "Family messages"
    public const string NoMessages = nameof(NoMessages);          // "No new messages."
    public const string Settings_MaxMessages = nameof(Settings_MaxMessages);

    private MessagesStrings() { }
}
```

Then `MessagesStrings.resx` (English, the neutral file), `MessagesStrings.es.resx` and `MessagesStrings.el.resx` with the same keys. Every display language must be complete; the localization test fails the build otherwise. Never build a sentence from parts. If a sentence needs a name or a number, use a placeholder and let each language place it: `"{0} is visiting at {1}."`

Widgets read strings for the **display's** language, not the browser's. Inside a view you get them through the base class:

```csharp
var t = Strings<MessagesStrings>();
var text = t[MessagesStrings.NoMessages];
```

### 4. Settings

A plain class with a parameterless constructor and sensible defaults. The portal generates a form from it: `bool` becomes a switch, `int` and `double` become numeric fields bounded by `[Range]`, enums become selects, `string` becomes a text field, and `LocalizedText` becomes a multi-language editor. `[Display(Name = ...)]` is treated as a key in your strings resource, so labels come out in the portal's language.

```csharp
using System.ComponentModel.DataAnnotations;

public sealed class MessagesSettings
{
    [Display(Name = nameof(MessagesStrings.Settings_MaxMessages))]
    [Range(1, 10)]
    public int MaxMessages { get; set; } = 3;
}
```

Settings are stored as JSON on the placement and deserialized by type. Bad or missing JSON yields a default instance, never an error. Settings travel to the display, so never put a secret in them; a secret belongs on the server behind an interface (see how the calendar's feed address is handled through `ICalendarFeedStore`).

If a property needs a custom editor, for example a picker that lists the household's albums, register one from a Portal project (step 9). The generated form uses it in place of the default field.

### 5. Data

If the widget needs outside data, define an immutable record for what the display receives. It must be serializable with `System.Text.Json`. Use `LocalizedText` for anything a family member typed, so the display can resolve it in its language.

```csharp
public sealed record MessagesData(IReadOnlyList<MessageItem> Messages)
{
    public static MessagesData Empty { get; } = new([]);
}

public sealed record MessageItem(Guid Id, LocalizedText Text, DateTimeOffset PostedUtc);
```

### 6. The definition

```csharp
public sealed class MessagesWidgetDefinition : IWidgetDefinition
{
    public const string Key = "kinlight.messages";

    public string TypeKey => Key;
    public string NameResourceKey => nameof(MessagesStrings.WidgetName);
    public Type StringsType => typeof(MessagesStrings);
    public Type SettingsType => typeof(MessagesSettings);
    public Type ViewComponent => typeof(MessagesWidget);
    public Type? DataType => typeof(MessagesData);      // null for a widget without data
    public WidgetSize DefaultSize => new(6, 2);          // columns × rows on the 12 × 6 grid
    public WidgetSize MinSize => new(4, 1);

    // Made-up data for the layout editor's live preview and for development hosts. Never real people.
    public object? CreateSampleData(DisplayContext context) => new MessagesData(
    [
        new MessageItem(Guid.NewGuid(), LocalizedText.From(context.Culture.Name, "Nikos is visiting at 3."), context.Clock.GetUtcNow()),
    ]);
}
```

`CreateSampleData` is what the portal shows inside the editor tile, so make it look like real use. It receives the display's culture, time zone and clock through `DisplayContext`.

### 7. The view

Inherit `WidgetComponentBase<TSettings, TData>` (or `WidgetComponentBase<TSettings>` for a widget without data). You receive `Placement`, `Settings`, `Data` (the last good data, possibly null), and the cascading `Context`.

```razor
@inherits WidgetComponentBase<MessagesSettings, MessagesData>

<div class="kl-messages" lang="@Context.Culture.Name">
    @if (Visible.Count == 0)
    {
        <p class="kl-messages__empty">@T[MessagesStrings.NoMessages]</p>
    }
    else
    {
        <ul class="kl-messages__list">
            @foreach (var message in Visible)
            {
                <li class="kl-messages__item">@Context.Resolve(message.Text)</li>
            }
        </ul>
    }
</div>

@code {
    private IWidgetStrings T => Strings<MessagesStrings>();

    private IReadOnlyList<MessageItem> Visible =>
        (Data?.Messages ?? []).OrderByDescending(m => m.PostedUtc).Take(Settings.MaxMessages).ToList();
}
```

Rules for the view:

- **Render something sensible when `Data` is null** and never throw. Anything that does throw is caught by the error boundary around every widget and renders as nothing, which is the intended failure mode. Do not rely on it.
- **Format with the display's culture**, never the process culture: `value.ToString("t", Context.Culture)`. Day and month names come from `Context.Culture.DateTimeFormat`.
- **Resolve family-entered text through `Context.Resolve(text)`.** For safety-relevant text such as medication instructions, pass `allowUnreviewed: false` so an unreviewed machine translation is never shown.
- **Use `Context.LocalNow` and `Context.IsNight`**, not `DateTime.Now`.
- **No interaction.** No buttons, no links, no scrolling. If something needs confirming, it is confirmed in the portal.
- If the widget needs to re-render on a timer, own the timer and dispose it. The clock widget shows the pattern.

### 8. Styling

Put styles in a scoped file next to the view, `MessagesWidget.razor.css`. Two custom properties connect you to the display:

```css
.kl-messages {
    block-size: 100%;
    padding-inline: 0.5em;
    /* Base size follows the screen (2.2% of its height); the family scales it per widget. */
    font-size: calc(var(--kl-base-font, 1rem) * var(--font-scale, 1));
    line-height: 1.3;
}

.kl-messages__item {
    font-size: 1.5em;
}
```

Size everything in `em` relative to that root, so the per-widget text size slider and the portal's scaled previews both work. Use logical properties (`padding-inline`, `margin-block-end`) so right-to-left languages can be added later. No Tailwind utility classes inside widgets: Tailwind only scans the display project, not yours. High contrast is applied by the host on the containing cell, so prefer inheriting colors over setting them.

### 9. Registration

Add an extension method in the Client project:

```csharp
public static class MessagesWidgetServiceCollectionExtensions
{
    public static IServiceCollection AddKinLightMessagesWidget(this IServiceCollection services)
        => services.AddWidget<MessagesWidgetDefinition>();
}
```

Then make it a built-in widget in `Modules/Widgets/BuiltIn`: add a project reference to your Client and one line to `AddKinLightBuiltInWidgets()`. That single list is what both the display and the portal load, so they can never disagree about which widgets exist. Registration is explicit by design; nothing scans assemblies, which keeps WebAssembly trimming safe.

Finally, add your Client assembly as a `TrimmerRootAssembly` in any host (`KinLight/KinLight.Client` today), because settings and data are deserialized by runtime type.

### 10. The Api project (widgets with outside data)

```
Modules/Widgets/Messages/Api/KinLight.Modules.Widgets.Messages.Api.csproj
```

A plain class library (or one with the ASP.NET Core framework reference if it maps endpoints) that references your Client project. It holds:

- **An interface for whatever the host must supply**, such as storage. The widget never talks to a database directly; the host implements the interface, which keeps household isolation in one place.
- **A data provider**:

```csharp
public sealed class MessagesDataProvider(IMessageStore store) : IWidgetDataProvider
{
    public string TypeKey => MessagesWidgetDefinition.Key;
    public TimeSpan RefreshInterval => TimeSpan.FromMinutes(1);

    public async Task<object> FetchAsync(WidgetDataRequest request, CancellationToken cancellationToken)
    {
        var settings = (MessagesSettings)WidgetSettingsJson.Read(request.SettingsJson, typeof(MessagesSettings));
        var messages = await store.ListAsync(request.HouseholdId, cancellationToken);
        return new MessagesData(messages);
    }
}
```

The host runs providers on their interval, stores the last good result per placement, and pings the display. If `FetchAsync` throws, the previous data stays. Every request carries the `HouseholdId`; use it in every query.

- **A registration method**, `AddKinLightMessagesWidgetProvider()`, that calls `services.AddWidgetDataProvider<MessagesDataProvider>()`. Add a call to it in `Modules/Display/Api`.

For a development host without a server, implement the data in the host's fake store, as `LocalStorageKinLightStore` does for photos.

### 11. The Portal project (widgets that need a management page)

Only when settings are not enough. Create `Modules/Widgets/Messages/Portal/` as a Razor class library that references your Client project and `Modules/Portal/Client`. It may use MudBlazor because only the portal loads it. It can contribute:

- **Pages** with `@page "/portal/messages"` and `@layout PortalLayout`. The host adds the assembly to its router.
- **A navigation entry** by implementing `IPortalNavItem` and registering it with `services.AddPortalNavItem<MessagesNavItem>()`.
- **A custom settings editor** for one property, registered with `services.AddWidgetSettingsEditor<MessagesSettings>(nameof(MessagesSettings.SomeProperty), typeof(SomePicker))`. The editor component takes `object? Value`, `EventCallback<object?> ValueChanged` and `string Label`.
- **Its own strings** in a `.resx` family. Because the project path contains `Portal`, the localization test counts them as portal-side, so they must exist in every portal language.

The Photos widget is the reference implementation of all three.

### 12. Checklist before opening a pull request

- [ ] `TypeKey` is final. It will never change.
- [ ] The Client project references only Abstractions and Shared.
- [ ] Every string is in `.resx`, as a whole sentence, in every display language. `dotnet test` passes.
- [ ] The view renders with `Data == null` and never throws.
- [ ] All formatting uses `Context.Culture`; time uses `Context.LocalNow`.
- [ ] Sample data is invented and looks like real use.
- [ ] Nothing on the display is clickable.
- [ ] Settings carry no secrets.
- [ ] Added to `BuiltIn` and, if there is an Api project, to `Display/Api`.
- [ ] `TrimmerRootAssembly` added in the host.
- [ ] Any entity the widget owns on the server carries `HouseholdId` (see [ARCHITECTURE.md §9](ARCHITECTURE.md#9-multi-tenancy)).

---

## Adding a language

1. Copy every neutral `.resx` under `Modules/Widgets/**/Client` and `Modules/Display` to `Name.<culture>.resx` and translate it. Keep every key; sentences are whole, placeholders stay.
2. Run `dotnet test`. It lists exactly which files are missing or incomplete.
3. Add the culture to `KnownCultures.Display` in `Modules/Shared/KnownCultures.cs`. The test will refuse this step until step 1 is complete, and will insist on it once it is.
4. For the portal, do the same for the `.resx` files under `Modules/Portal` and under any `Modules/Widgets/*/Portal`, then add the culture to `KnownCultures.Portal`.
5. If the language uses a script the bundled fonts do not cover, vendor a subset of a suitable font into both `Modules/Display/Client/wwwroot/fonts` and `Modules/Portal/Client/wwwroot/fonts` with its license, and declare it with a `unicode-range` as the Greek Noto Sans subsets are declared.

Dates, day names and number formats need no work: they come from ICU, which the display loads in full.

---

## Contributing

Contributions are welcome, and translations especially. Please read [ARCHITECTURE.md](ARCHITECTURE.md) first, in particular §2 (sensitivity), §3 (display principles) and §22 (rules for contributors and AI assistants). In short:

- Never add a reference that breaks browser safety.
- Never change a widget's type key after release.
- The display must never show an error, an unresolved spinner, or a connection message.
- Every tenant-owned entity has a `HouseholdId`, a query filter and isolation test coverage.
- Every command declares which roles may run it and has a test proving the others cannot.
- Notifications never include medication names, doses or the person's name.
- Every schema change adds migrations for both SQLite and PostgreSQL.
- No telemetry, analytics or third-party scripts.
- Use made-up people, photos and medications everywhere.

A Contributor License Agreement will be in place before the first outside contribution is merged; see the license section for why.

---

## License

The license is **not yet final**. The recommended choice, pending legal review, is **AGPL-3.0-or-later** with a Contributor License Agreement. Under the AGPL families can self-host freely, and anyone who modifies KinLight and offers it to others as a network service must publish their changes. Families running KinLight unmodified have no obligations beyond keeping the license notice. The reasoning, and the alternative that was considered, are in [ARCHITECTURE.md §6](ARCHITECTURE.md#6-license).

Third-party components bundled or referenced:

| Component | Use | License |
|---|---|---|
| MudBlazor and MudBlazor.Translations | Portal UI components | MIT |
| gridstack.js 14 | Portal layout editor (vendored) | MIT |
| Tailwind CSS 4 | Display styling | MIT |
| Atkinson Hyperlegible Next | Display and portal font (vendored) | SIL Open Font License 1.1 |
| Noto Sans, Greek subsets | Greek glyphs (vendored) | SIL Open Font License 1.1 |
| Ical.Net | Calendar feed parsing | MIT |
| DispatchR, Npgsql EF Core provider | Planned (mediator, PostgreSQL) | To be verified before use |
