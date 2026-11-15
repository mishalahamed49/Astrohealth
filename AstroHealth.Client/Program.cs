using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AstroHealth.Client;
using AstroHealth.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Use configured ApiBaseUrl for local dev (e.g. https://localhost:7189/)
// Fall back to the app's own host base address when deployed (Render, Azure, etc.)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
var baseAddress = string.IsNullOrWhiteSpace(apiBaseUrl) || apiBaseUrl == "/"
    ? builder.HostEnvironment.BaseAddress
    : apiBaseUrl;

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });
builder.Services.AddScoped<HealthApiService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<CrewProfileService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
