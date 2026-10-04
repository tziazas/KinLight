using KinLight.Client;
using KinLight.Client.Fakes;
using KinLight.Modules.Display.Client;
using KinLight.Modules.Portal.Client;
using KinLight.Modules.Widgets.Medication.Client;
using KinLight.Modules.Widgets.Medication.Portal;
using KinLight.Modules.Widgets.Photos.Client;
using KinLight.Modules.Widgets.Photos.Portal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddKinLightDisplayClient();
builder.Services.AddKinLightPortalClient();
builder.Services.AddKinLightPhotosPortal();
builder.Services.AddKinLightMedicationPortal();

// Development stand-in for the signed-in member, switchable from the app bar.
builder.Services.AddSingleton<FakeCurrentMember>();
builder.Services.AddSingleton<ICurrentMember>(sp => sp.GetRequiredService<FakeCurrentMember>());
builder.Services.AddPortalAppBarItem<MemberSwitcher>();

// One fake store, backed by localStorage, serves both sides so a save in the portal shows on the display.
builder.Services.AddSingleton<IndexedDbPhotoLibrary>();
builder.Services.AddSingleton<IPhotoLibraryApi>(sp => sp.GetRequiredService<IndexedDbPhotoLibrary>());
builder.Services.AddSingleton<LocalStorageMedicationStore>();
builder.Services.AddSingleton<IMedicationApi>(sp => sp.GetRequiredService<LocalStorageMedicationStore>());
builder.Services.AddSingleton<LocalStorageKinLightStore>();
builder.Services.AddSingleton<IDisplayApi>(sp => sp.GetRequiredService<LocalStorageKinLightStore>());
builder.Services.AddSingleton<IPortalApi>(sp => sp.GetRequiredService<LocalStorageKinLightStore>());

// The boot culture is set by index.html from the cached display config before Blazor starts (ARCHITECTURE.md §11).
var host = builder.Build();
await host.Services.GetRequiredService<FakeCurrentMember>().LoadAsync();
await host.RunAsync();
