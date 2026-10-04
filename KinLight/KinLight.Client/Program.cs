using KinLight.Client;
using KinLight.Client.Fakes;
using KinLight.Modules.Display.Client;
using KinLight.Modules.Portal.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddKinLightDisplayClient();
builder.Services.AddKinLightPortalClient();

// One fake store, backed by localStorage, serves both sides so a save in the portal shows on the display.
builder.Services.AddSingleton<LocalStorageKinLightStore>();
builder.Services.AddSingleton<IDisplayApi>(sp => sp.GetRequiredService<LocalStorageKinLightStore>());
builder.Services.AddSingleton<IPortalApi>(sp => sp.GetRequiredService<LocalStorageKinLightStore>());

// The boot culture is set by index.html from the cached display config before Blazor starts (ARCHITECTURE.md §11).
await builder.Build().RunAsync();
