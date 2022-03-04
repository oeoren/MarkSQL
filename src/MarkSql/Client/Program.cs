using MarkSql.Client;
using MarkSql.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
var localApiClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var localApiService = new LocalApi(localApiClient);
builder.Services.AddSingleton(localApiService);

await builder.Build().RunAsync();
