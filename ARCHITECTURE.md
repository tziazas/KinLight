# KinLight Architecture

## Why KinLight exists

Watching someone you love lose track of the day, the time, and even the faces around them is one of the hardest parts of Alzheimer's and other dementias. Small, steady reminders help: what day it is, whether it's morning or evening, who is in a photo and how they're related, what's happening today.

KinLight exists to help people whose relatives are living with Alzheimer's or dementia. It turns a Raspberry Pi with a screen, or an old tablet, into a calm, always-on display the person can glance at, while family members, near or far, keep it up to date from their phones. It is open source and free to run yourself, with an optional low-cost hosted version at kinlight.care for families who don't want to set up hardware.

---

## 1. What KinLight does

KinLight has two faces:

**The display** is what the person living with dementia sees. It is a full-screen board of widgets that needs no touching, no logging in, and never shows an error. Widgets include a clock that says the day in plain words ("It is Saturday afternoon."), today's calendar, family photos with names and relationships ("Maria, your daughter"), medication reminders, and messages from family. At night the screen dims and says it's night-time. If the internet or server goes away, the display keeps showing what it last knew.

**The portal** is what family members and caregivers use, mostly on a phone. Family members arrange widgets on a picture of the actual screen, make text bigger or smaller per widget, choose the display's language and time zone, set night hours, add photos and messages, pair new screens, and invite others. Caregivers, such as home aides, see a simpler Today page where they confirm medication they've given. Saved changes reach the screen within seconds.

---

## 2. Sensitivity and responsibility

KinLight holds some of the most personal information a family has, about a person who may not be able to understand or consent to how it's used:

- photos of family members, with names and relationships;
- calendars showing appointments and when the person is home alone;
- medication names, doses and schedules, which are health information;
- the secret iCal address of a Google Calendar, which gives read access to that calendar to anyone who has it.

Every contributor, human or AI, must treat this as a first-class requirement, not an afterthought.

**Rules that follow from this:**

1. **Collect the minimum.** No field exists unless a feature needs it. No analytics, trackers, advertising, or third-party scripts in the display or the portal.
2. **Isolation is absolute.** One household must never see another household's data. This is enforced in three places (see [Multi-tenancy](#9-multi-tenancy)) and covered by a test that must never be skipped.
3. **Secrets are stored as secrets.** Calendar addresses, device tokens and API keys are encrypted at rest (ASP.NET Core Data Protection) or stored only as hashes.
4. **Families own their data.** One-click export of everything, and full deletion, in both editions.
5. **Safety-relevant text is never machine-translated without review.** Medication instructions fall back to the original language rather than show an unchecked translation.
6. **Medication reminders are a supplement, never the safety mechanism.** The person living with dementia is never asked to confirm a dose: someone with memory impairment may confirm without taking it, or take it twice. The caregiver who gives the medication confirms it. The product and its documentation must say so plainly.
7. **Test with made-up people.** Never use real family photos, names or medications in development, tests, bug reports, screenshots, or AI coding tools.
8. **The display is not exposed to the public internet without pairing and HTTPS.** Self-hosters are pointed to Tailscale (or similar) rather than port forwarding.

**Legal (not legal advice; to be reviewed by a lawyer before the hosted edition charges anyone):**

- HIPAA generally applies to healthcare providers, insurers and their business associates, not to a product a family buys directly. It could apply if the hosted edition is sold to care homes.
- Health apps outside HIPAA can still fall under the FTC Health Breach Notification Rule and state consumer health data laws.
- For users in the EU, health information is special-category data under GDPR.
- A plain-language privacy policy is required before launch of the hosted edition.

---

## 3. Design principles for the display

These come from how dementia affects perception and memory. They override visual preferences.

| Principle | What it means in code |
|---|---|
| Never show an error | Widgets render inside an `ErrorBoundary` with empty error content. Failed refreshes keep the last good data. The Blazor error bar is removed from `index.html`. |
| Plain language | "It is Saturday afternoon." rather than "Sat 14:32". Dates are written out, never abbreviated. |
| Whole-sentence translations | Resource strings are complete sentences per case. Sentences are never assembled from fragments, because word order and grammar differ between languages. |
| No interaction required | Nothing on the display ever requires or offers a touch: many displays are TVs or monitors with no touchscreen. No scrolling, no popups, no login, no buttons. Anything that needs confirming is confirmed by a caregiver in the portal. |
| Stable and calm | Fixed layout, minimal animation, no content that moves unexpectedly. Respects `prefers-reduced-motion`. |
| Upcoming, not past | Calendars show what's next today, not what already happened. |
| Relationships, not just names | Photo captions say who the person is to the viewer. |
| Night mode | Between the configured hours the screen dims and says it's night-time, so a glance at 3 a.m. doesn't prompt getting dressed. Setting start and end to the same hour turns it off. |
| The person's first language | Bilingual people often lose their later-learned language first as dementia progresses. The display language is set per display, independent of the device and of the portal. |
| Legibility | Atkinson Hyperlegible Next (designed by the Braille Institute for low-vision readers) is bundled locally. Per-widget text size and a high-contrast option. |

---

## 4. Editions

KinLight runs the same code in two editions.

| | Self-hosted (free, open source) | Hosted at kinlight.care (paid) |
|---|---|---|
| Who it's for | Families comfortable setting up a Raspberry Pi or small server | Families who want it to just work |
| Runs on | Raspberry Pi (Docker), any Linux/Windows/macOS machine | Our cloud infrastructure |
| Database | SQLite in a data folder | PostgreSQL |
| Photos and files | Local disk | Azure Blob Storage |
| Households | One by default; more are allowed | Many |
| Sign-up | First run creates the owner account | Self-service sign-up with subscription |
| Billing | None | Stripe (subscription, small annual price, TBD) |
| Updates and backups | The family's responsibility | Ours |
| Remote access | Tailscale recommended | Built in, HTTPS |

**The hosted edition is the open-source KinLight plus billing and operations.** Everything that protects a family's data (accounts, pairing, isolation) is in the open-source code, where anyone can inspect it.

**Export promise:** hosted families can export everything at any time and move to a self-hosted install. If the hosted service ever shuts down, that path remains.

---

## 5. Repositories

| Repository | Visibility | Contents |
|---|---|---|
| `kinlight` | Public | Everything in this document except section 13. Published as NuGet packages under the `KinLight` prefix. |
| `kinlight-cloud` | Private | The kinlight.care host, billing, sign-up, marketing site, operator tools, infrastructure and deployment. |

**Code flows one way only: public into private.** The private repository composes the public packages; it never forks them. Nothing from the private repository is copied into the public one. If something written privately would benefit self-hosters, it is deliberately moved to the public repository.

---

## 6. License

**Recommended: AGPL-3.0-or-later for the public repository, with a Contributor License Agreement (CLA).** Final decision pending legal review.

Why AGPL: families can self-host freely, and anyone who modifies KinLight and offers it to others as a network service must publish their changes. This prevents a third party from taking KinLight, closing it, and selling it. Families running KinLight unmodified have no obligations beyond keeping the license notice.

Why a CLA is required: the copyright holder is not bound by their own license, which is what allows the private `kinlight-cloud` code to be combined with KinLight. That only holds for code the maintainer owns. Outside contributions are licensed to the project under AGPL, so without a CLA the hosted edition would contain AGPL code the maintainer can't combine with private code. **The CLA (for example via the CLA Assistant GitHub app) must be in place before the first outside contribution is merged.**

The alternative considered was MIT: simpler, no CLA needed, but anyone could run a closed, paid copy of KinLight.

**Third-party components and their licenses** (keep this list current):

| Component | Use | License |
|---|---|---|
| MudBlazor | Portal UI components | MIT |
| gridstack.js 14 | Portal layout editor (vendored) | MIT |
| Atkinson Hyperlegible Next | Display and portal font (vendored) | SIL Open Font License |
| DispatchR (hasanxdev) | Mediator | Verify before first release |
| Ical.Net | Calendar widget | Verify before first release |
| Npgsql EF Core provider | PostgreSQL | PostgreSQL License |

---

## 7. Technology stack

| Area | Choice | Notes |
|---|---|---|
| Runtime | .NET 10 (LTS) | |
| Display | Blazor WebAssembly, installable PWA | Offline-first. No Blazor Server here: its reconnect overlay would appear on the screen. |
| Portal (family and caregivers) | Blazor Server (interactive server) in a Razor class library | Calls Core in-process through DispatchR. |
| Portal UI | MudBlazor 9 | No Tailwind: its reset conflicts with MudBlazor, and it duplicates MudBlazor's utility classes. |
| Display styling | Scoped CSS + CSS custom properties | No component library and no Tailwind. Tailwind can't see class names in widget projects it doesn't scan. |
| API | ASP.NET Core minimal APIs | |
| Real-time | SignalR | Pings only; data is always fetched from the API. |
| Mediator | DispatchR by hasanxdev | Not the unrelated RoyceLark.DispatchR package. |
| Data | EF Core, provider-agnostic | SQLite (self-hosted), PostgreSQL (hosted). |
| Identity | ASP.NET Core Identity | Email + password, passkeys, household invitations, three roles (section 13). |
| Notifications | Web Push, email fallback | Medication due and missed-dose alerts to phones, no app store app. |
| Layout editor | gridstack.js | Vendored, loaded from the server; no CDN. |
| Calendar | Ical.Net | Parses Google Calendar's secret iCal address. No OAuth. |
| Containers | Docker, standard `aspnet:10.0` image | Not Alpine or chiseled: they lack ICU, so dates silently fall back to English. |
| Solution file | `.slnx` | |

---

## 8. Solution structure

### Projects

```
kinlight/
  KinLight.slnx
  src/
    KinLight.Shared               Contracts that cross the wire. Browser-safe. No dependencies.
    KinLight.Widgets              Widget contracts and built-in widgets (views, settings, strings). Browser-safe.
    KinLight.Core                 Business logic: DispatchR commands/handlers/behaviors, EF Core model,
                                  tenancy, widget data providers, extension-point interfaces.
    KinLight.Client.Display       Blazor WebAssembly PWA: the screen.
    KinLight.Client.Portal        Razor class library: the portal for family and caregivers
                                  (Blazor Server, MudBlazor).
    KinLight                      ASP.NET Core integration: AddKinLight()/MapKinLight(), minimal API
                                  endpoints, SignalR hub, authentication, hosting of portal and display.
                                  The package families and the private host install.
    KinLight.Migrations.Sqlite    EF Core migrations for SQLite.
    KinLight.Migrations.Postgres  EF Core migrations for PostgreSQL.
    KinLight.Server               The self-hosted executable. A few lines of Program.cs, appsettings,
                                  Dockerfile. Not published to NuGet.
  tests/
    KinLight.Core.Tests           Includes the household isolation tests.
    KinLight.Widgets.Tests
    KinLight.Tests                Endpoint and pairing integration tests (WebApplicationFactory).
  docs/
  Dockerfile
  compose.yaml
```

### What each project may reference

References only point inward. Nothing that runs in the browser ever references Core.

```
KinLight.Client.Display  ──► KinLight.Widgets ──► KinLight.Shared
KinLight.Core            ──► KinLight.Widgets, KinLight.Shared
KinLight.Client.Portal   ──► KinLight.Core, KinLight.Widgets, KinLight.Shared
KinLight                 ──► all of the above + both Migrations projects
KinLight.Server          ──► KinLight
```

| Project | Must be browser-safe | Must not reference |
|---|---|---|
| Shared | Yes | DispatchR, EF Core, ASP.NET Core server packages, any other KinLight project |
| Widgets | Yes | Core, EF Core, server packages |
| Core | No | ASP.NET Core hosting, SignalR server, MudBlazor, Client projects |
| Client.Display | Yes | Core, anything server-only |
| Client.Portal | No | Client.Display |

### Where things live

| Thing | Project | Why |
|---|---|---|
| `DisplayConfig`, `WidgetInstance`, `LocalizedText`, pairing DTOs, `BoardGrid`, `NightMode` | Shared | Used by both the browser and the server. |
| Hub contract (`IDisplayClient`, hub path, method names) | Shared | The display needs it to connect. |
| `DisplayHub` and the SignalR implementation of `IDisplayNotifier` | KinLight | Hubs depend on the ASP.NET Core server stack. |
| `IDisplayNotifier` interface | Core | Handlers can notify a display without knowing SignalR exists. |
| Commands, queries, handlers, pipeline behaviors | Core | DispatchR request types implement DispatchR interfaces, so they can't be in Shared. |
| `IWidgetDefinition`, `WidgetComponentBase`, widget views and settings | Widgets | Rendered in the browser. |
| Widget data providers (e.g. fetching and parsing a calendar feed) | Core | Server-side work, often with server-only libraries. |
| `DbContext`, entities | Core | Provider-agnostic. |
| Migrations | Migrations.Sqlite / Migrations.Postgres | EF Core needs one migrations assembly per provider. |

---

## 9. Multi-tenancy

A **household** is the tenant. Self-hosted installs have one household by default; the hosted edition has many.

Every tenant-owned entity carries a `HouseholdId`. Isolation is enforced in three layers, so a mistake in one is caught by another:

1. **EF Core global query filters** on every tenant-owned entity, driven by a scoped `ICurrentHousehold`.
2. **A DispatchR pipeline behavior** that verifies, before any handler runs, that the signed-in member belongs to the household a command targets *and* that their role is allowed to run it (see [Roles](#roles)). It can't be forgotten in an individual handler.
3. **Isolation tests** in `KinLight.Core.Tests` that create two households and assert that no query, command, file, or endpoint in one can reach the other. These tests are never skipped or weakened.

Display devices are bound to one display in one household. A device token can never be used to read another display's data.

---

## 10. Domain model

| Entity | Key fields | Notes |
|---|---|---|
| `Household` | Id, Name, CreatedUtc | The tenant. |
| `Member` | Identity user + `HouseholdId`, Role (`Owner`, `Family`, `Caregiver`), AccessExpiresUtc | ASP.NET Core Identity. One person per account, never shared. Invited by link or email. Optional access expiry for caregivers. |
| `Display` | Id, HouseholdId, Name, Culture, FallbackCulture, TimeZoneId (IANA), NightStartsAt, NightEndsAt, ScreenShape, Version | `Version` is a concurrency token bumped on every save. |
| `WidgetPlacement` | Id, DisplayId, TypeKey, X, Y, Width, Height, FontScale, HighContrast, IsEnabled, VisibleFrom/To, SettingsJson | Grid is fixed at 12 columns × 6 rows (`BoardGrid`). |
| `DisplayDevice` | Id, DisplayId, Name, TokenHash, PairedUtc, LastSeenUtc, RevokedUtc | One per physical screen. |
| `PairingSession` | Code, DeviceSecretHash, ExpiresUtc, ClaimedDisplayId | Short-lived. |
| `PushSubscription` | MemberId, endpoint, keys, CreatedUtc | One per phone or browser a member enables notifications on. |
| Widget-owned data | e.g. `Photo`, `Message`, `MedicationSchedule`, `MedicationConfirmation` (dose, ConfirmedByMemberId, ConfirmedUtc) | Always carry `HouseholdId`. |

Ids are created in code (`Guid.NewGuid()`) and configured with `ValueGeneratedNever()`. New entities are added explicitly with `DbSet.Add`, because EF would otherwise treat a new entity with a preset key as an existing one.

---

## 11. The display (KinLight.Client.Display)

**Startup:**

1. Load the display's config from the API. If the server is unreachable, load the last good copy from `localStorage`. If neither exists, show a clock-only layout.
2. Set the WebAssembly culture from the config's display language. Culture is fixed at startup, so a later language change triggers a page reload.
3. Connect to SignalR and join the display's group.

**Updates:** the server sends a `ConfigChanged` ping; the display fetches the new config and updates in place (or reloads if the language changed). After any reconnect the display fetches again, in case it missed a ping. The connection retries forever, quietly.

**Offline:** the service worker caches the app, including `.woff2` fonts. It must never answer `/portal` navigations with the cached display. Widget data is cached as its last good version; a failed refresh never clears it.

**Required settings:**

- `BlazorWebAssemblyLoadAllGlobalizationData=true`: the display language comes from the portal, not the browser.
- `TrimmerRootAssembly` for widget assemblies: widget settings are deserialized by runtime type.
- Time zones are IANA ids, understood by both Linux and browsers.

**Widget hosting:** `WidgetHost` places each widget on the CSS grid, exposes `--font-scale`, deserializes settings (falling back to defaults on bad JSON), renders an unknown widget type as nothing, and wraps the widget in an `ErrorBoundary` that shows nothing and recovers when new data arrives.

---

## 12. Display pairing and security

The display has no sign-in, so it authenticates as a **paired device**, like pairing a streaming app on a TV. This applies to both editions.

1. A new display asks the server for a pairing session and shows a six-digit code: "Pair this screen: 4 8 2 7 1 5". It holds a random device secret it never displays.
2. An Owner signed in to the portal enters the code and chooses which display this screen is.
3. The display, polling with its secret, receives a long random **device token** and stores it.
4. Every API request and the SignalR connection carry the token (as a bearer token; for WebSockets, the `access_token` query parameter). The server stores only its hash.
5. Caregivers can see paired devices, rename them, and revoke a lost or replaced tablet.

Codes expire after a few minutes, are single-use, and the pairing endpoints are rate limited.

**Portal authentication:** ASP.NET Core Identity with cookie authentication, passkeys, and invitation links; roles as described in section 13. Sign-in and first-run setup pages are static server-rendered forms, because they set the auth cookie. Data Protection keys are persisted to the data folder so restarts and updates don't sign everyone out. Self-hosters without email configured can reset a password from the command line.

---

## 13. The portal (KinLight.Client.Portal)

The portal is the app family members and caregivers use, mostly on phones. It is a Razor class library of Blazor Server pages using MudBlazor, hosted by the `KinLight` package at `/portal`. Pages send DispatchR commands to Core in-process; there are no HTTP endpoints between the portal and Core. If a WebAssembly version or a phone app is ever wanted, each endpoint becomes a thin wrapper around a command that already exists.

### Roles

Every household member has exactly one role. Roles are enforced by the DispatchR household authorization behavior, not just by hiding navigation: a member can't run a command their role doesn't allow, even by calling it directly.

| Role | Typically | Can do |
|---|---|---|
| **Owner** | The family member who set KinLight up. A household can have more than one. | Everything: members and roles, paired devices, displays, all content, export and deletion of the household, and billing (hosted edition). |
| **Family** | Relatives helping out | Photos, messages, calendars, medication schedules, layout and display settings, confirming medication. Not: members, devices, export and deletion, billing. |
| **Caregiver** | Home aides, agency staff, a neighbor who helps | The Today page only: today's schedule, confirming medication they've given, and posting a short message to the display. No access to the photo library, layout, settings, medication schedules, or other members. |

**Accounts are always per person, never shared.** The medication log records who confirmed each dose, and removing someone never affects anyone else's sign-in. Caregivers join through an invitation link and sign in with a passkey (or email and password), so an aide isn't managing yet another password. Owners can set an **access expiry date** on a caregiver, for example the end of an agency assignment, after which access ends automatically.

### Pages

| Page | Roles | Purpose |
|---|---|---|
| **Today** | All (landing page for Caregivers) | Today's medications with a large Confirm button per dose; what's happening today; a short-message box. Confirming a dose takes one tap. |
| **Displays** | Owner, Family | The household's displays. |
| **Arrange** | Owner, Family | The layout editor, framed as the actual screen in its real shape (16:9, 16:10, 4:3). gridstack.js owns the grid's DOM; Blazor owns the model; they communicate through a small JS module. Drag to move, drag the corner to resize, select to change text size, high contrast, visibility, or remove. Nothing reaches the screen until **Save**; leaving with unsaved changes asks for confirmation. Saves are guarded by the display's `Version`. |
| **Settings** | Owner, Family | Name, screen shape, display language, backup language, time zone, night hours. |
| **Widget settings** | Owner, Family | Forms generated from each widget's settings class (DataAnnotations for labels and ranges). `LocalizedText` properties get a multi-language editor with a Translate button. |
| **Medications** | Owner, Family | Medication schedules and dose windows, and the confirmation history (who, when). |
| **Devices** | Owner | Pair a new screen, rename, revoke. |
| **Members** | Owner | Invite by link or email, set roles and access expiry, remove access. |
| **Export and delete** | Owner | Download everything; delete the household. |
| **Notifications** | All | Turn phone notifications on or off for this device. |

The navigation shows only the pages a member's role allows.

### Notifications

When a dose is due, Caregivers (and Family members who opt in) get a notification on their phone. If a dose isn't confirmed by the end of its window, Owners and Family members are alerted.

Notifications use **Web Push** to the portal saved to the phone's home screen, which works on iPhone and Android without an app store app, with email as a fallback. The portal has a small service worker used only for push, not for offline caching.

**Notification text never contains medication names, doses, or the person's name**, because lock-screen previews can be seen by anyone nearby. It says only that a reminder needs attention and links to the Today page.

### Visual design

Calm and plain, for family members and caregivers on phones. Harbor blue `#2F5D7C` primary, sage `#5E7F63`, amber `#B7791F` for unsaved changes, cool off-white background, Atkinson Hyperlegible Next. The layout editor's device bezel is the one distinctive element. The Today page is designed for one-handed use: large touch targets and nothing to scroll past before the next dose.

### Extension points for other hosts

Used by the private repository: additional assemblies for routing, so a host can add its own portal pages (such as Billing); and an `IPortalNavItem` service, including the roles allowed to see each item, so those pages appear in the navigation without the public code knowing about them.

---

## 14. Widgets (KinLight.Widgets)

**Contract:**

- `IWidgetDefinition`: `TypeKey` (stable; stored in the database and never changed after release), `NameResourceKey`, `StringsType`, `SettingsType`, `ViewComponent`, `DataType`, `DefaultSize`, `MinSize`.
- `WidgetComponentBase<TSettings, TData>`: receives the placement, settings, last good data, and a cascading `DisplayContext` (culture, fallback culture, time zone, clock, night mode).
- `IWidgetDataProvider` (in Core): fetches outside data on a schedule, stores the last good result, and pings the display. A failed fetch keeps the old data.

Widgets are registered explicitly (`AddWidget<ClockWidgetDefinition>()`), not by assembly scanning. That's trim-safe in WebAssembly and makes adding or removing a feature a one-line change.

**Built-in widgets:**

| Widget | Status | Notes |
|---|---|---|
| Clock and day | Prototype done | Day-part sentence, time, long date. Night sentence omits the weekday. |
| Calendar | Planned | Google Calendar secret iCal address (no OAuth), parsed server-side with Ical.Net. Shows upcoming events today in plain language. The address is stored encrypted. |
| Photos | Planned | Uploaded in the portal, with `LocalizedText` captions including relationships. Google restricted its Photos Library API in 2025, so no Google Photos sync. Immich integration possible later. |
| Medication reminders | Planned | The display shows a large reminder during the dose window, with no button. The caregiver who gives the dose confirms it with one tap on the portal's Today page on their phone, prompted by a push notification when the dose is due; the log records who and when. Once confirmed, the display shows e.g. "Morning medication: taken at 8:10." (wording to be validated with a dementia-care professional). Other household members are alerted if a dose isn't confirmed by the end of its window. Never shows unreviewed translations. Documented as a supplement only. |
| Family messages | Planned | Short messages from family, e.g. "Nikos is visiting at 3." |
| Weather | Later | Plain language: "Cold today, wear a coat." |

If third-party widgets become common, the contracts move to a small `KinLight.Widgets.Abstractions` package so outside authors don't depend on the built-in widgets.

---

## 15. Localization

Two separate problems:

1. **Built-in text** (sentences, labels): `.resx` resources through `IStringLocalizer`, one whole sentence per case. Day and month names come from `CultureInfo`. Languages are added by translating resource files; community translations are welcome.
2. **Text caregivers enter** (captions, messages, medication instructions): `LocalizedText`, a map of culture to text plus the set of cultures that were machine-translated and not yet reviewed. It resolves through the culture's parents, then the backup language, then any reviewed value. Safety-relevant widgets resolve with unreviewed translations disallowed.

Machine translation is optional and pluggable through `ITranslationProvider` (Azure AI Translator, DeepL, self-hosted LibreTranslate). It only fills in missing languages and never overwrites text a person wrote or reviewed.

The display language, the backup language, the portal's language, and the device's language are all independent.

Operational notes: Docker images must include ICU. The bundled font covers Latin scripts; other scripts fall back to Noto Sans or the system font. CSS uses logical properties (`margin-inline-start`) so right-to-left languages can be added cheaply.

---

## 16. DispatchR usage

- Package: **DispatchR by hasanxdev**. Handlers return `ValueTask`.
- Features are organized as vertical slices in Core: `Features/Displays/SaveLayout.cs` contains the command, its handler, and its validation together.
- Pipeline behaviors use DispatchR's chain-of-responsibility style (an injected `NextPipeline`, not a `next()` delegate). Order: logging → household authorization → validation → handler.
- Commands and queries never live in Shared.

---

## 17. Data and storage

- One provider-agnostic `DbContext` in Core; migrations in `KinLight.Migrations.Sqlite` and `KinLight.Migrations.Postgres`. Every schema change adds a migration to both.
- Self-hosted: everything that must survive an update lives in one data folder: the SQLite database, uploaded files, and Data Protection keys. Backing up is copying that folder.
- Files go through `IFileStore`: local disk in the public repository, Azure Blob Storage in the private one. Files are served only through authorized endpoints, never as public static files.
- Concurrency: `Display.Version` guards against two portal windows overwriting each other.

---

## 18. The KinLight NuGet package

**Installing the `KinLight` package brings in everything needed to run KinLight**: Shared, Widgets, Core, Client.Portal, Client.Display, both migrations packages, and the ASP.NET Core integration. A host needs only:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddKinLight(options =>
{
    options.UseSqlite(dataDirectory: "data");   // or options.UsePostgres(connectionString)
});

var app = builder.Build();
app.MapKinLight();   // display, portal, API, SignalR hub
app.Run();
```

`KinLight.Server` in the public repository is exactly this, plus configuration and a Dockerfile.

**Published packages:**

| Package | Contents |
|---|---|
| `KinLight` | ASP.NET Core integration; depends on all packages below |
| `KinLight.Shared` | Wire contracts |
| `KinLight.Widgets` | Widget contracts and built-in widgets |
| `KinLight.Core` | Business logic and data model |
| `KinLight.Client.Portal` | The portal for family and caregivers |
| `KinLight.Client.Display` | The compiled display app as static assets |
| `KinLight.Migrations.Sqlite` | SQLite migrations |
| `KinLight.Migrations.Postgres` | PostgreSQL migrations |

**Rules:**

- Reserve the `KinLight.` package ID prefix on nuget.org.
- The public extension points (`AddKinLight`, `MapKinLight`, their options, the Core interfaces, the portal extension points) are a public API and follow semantic versioning. A minor release never breaks a host.
- Packages are published from CI on a version tag, with SourceLink.

**Known risk, to prove out early:** a Blazor WebAssembly *app* isn't a normal library package. Shipping `KinLight.Client.Display` means packaging its published `wwwroot` (including `_framework`) as static web assets and serving it at the site root from `MapKinLight()`. Build a small spike of this before relying on it. Fallback: the private repository references the public projects through a git submodule instead of packages.

**Development workflow:** the private repository includes the public one as a git submodule. A `Directory.Build.props` switch chooses between project references (development) and package references (CI and releases).

---

## 19. The private repository (kinlight-cloud)

Not part of the public project. Listed here only to define the boundary.

```
kinlight-cloud/
  src/
    KinLight.Cloud.Server     Host: builder.AddKinLight(...).AddKinLightCloud(); app.MapKinLight();
    KinLight.Cloud.Billing    Stripe Checkout, customer portal, webhooks, plan limits
    KinLight.Cloud.Storage    IFileStore on Azure Blob Storage
    KinLight.Cloud.Web        kinlight.care website and sign-up
    KinLight.Cloud.Operator   Internal support and operations tools
  infra/                      Infrastructure as code, deployment
  external/kinlight/          Public repository as a submodule (development)
```

It supplies hosted implementations of Core's extension points (`IFileStore`, `IEmailSender`, `IPlanLimits`), adds portal pages such as Billing through the portal extension points, and builds in CI against released public package versions. Card details never touch KinLight servers; Stripe handles them.

---

## 20. Deployment (self-hosted)

- **Docker:** `docker compose up -d --build`. Standard `mcr.microsoft.com/dotnet/aspnet:10.0` image (has ICU), non-root user, data in a named volume, `TZ` set for the default time zone of new displays.
- **Raspberry Pi kiosk:** 64-bit Raspberry Pi OS; Chromium in kiosk mode from the labwc autostart file, pointed at `http://localhost:8080/`.
- **Remote access to the portal:** Tailscale, which also provides HTTPS certificates (needed for the service worker on devices other than the Pi). Never forward the port to the internet.
- **Password reset without email:** a command-line flag on the server.

---

## 21. Status and roadmap

**Prototype (to be migrated into the structure above):** clock widget with English and Spanish strings; display shell with offline config cache, SignalR client, widget host and night mode; `LocalizedText` and translation hook; portal layout editor with gridstack, per-widget text size, settings page, and single-password sign-in (to be replaced by Identity accounts with roles). The prototype still uses the old name `Client.Admin` and the `/admin` path; both become `Client.Portal` and `/portal`.

**Order of work:**

1. Solution skeleton with the project structure and reference rules above; the NuGet display-packaging spike.
2. Households, Identity accounts, the three roles, invitation links, access expiry, and display pairing, with isolation and role tests.
3. Generated widget settings forms; `LocalizedText` editor and translation providers.
4. Calendar widget.
5. Photos widget and `IFileStore`.
6. Medication reminders: the Today page, caregiver confirmation, Web Push notifications, and missed-dose alerts to the household.
7. Family messages, then weather.
8. Export and delete.
9. NuGet publishing; then the hosted edition in the private repository.

**Open decisions:**

- Final license (AGPL + CLA recommended) after legal review; CLA in place before the first outside contribution.
- USPTO trademark search for "KinLight".
- Legal review of privacy obligations before the hosted edition charges anyone.
- Hosted pricing.
- License check of DispatchR and Ical.Net.

---

## 22. Rules for contributors and AI assistants

1. Read sections 2 and 3 before changing anything the display shows.
2. Never add a reference that breaks the browser-safety rules in section 8.
3. Never change a widget's `TypeKey` after release.
4. The display must never show an error, a spinner that doesn't resolve, or a connection message.
5. Every tenant-owned entity has a `HouseholdId`, a query filter, and isolation test coverage.
6. Every command declares which roles may run it, and has a test proving the other roles can't. Every portal page declares its roles.
7. Notifications never include medication names, doses, or the person's name.
8. Every schema change adds migrations for both SQLite and PostgreSQL.
9. No telemetry, analytics or third-party scripts.
10. Use made-up people, photos and medications in all development and tests.
11. Public extension points follow semantic versioning.
12. Nothing from the private repository is ever copied into this one.
