using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AstroHealth.Client;
using AstroHealth.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl must be configured in wwwroot/appsettings.json.");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<HealthApiService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<CrewProfileService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
