using MarkSql.ClientLib;
using MarkSql.Client;
using MarkSql.Client.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//var localApiClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
//var localApiService = new LocalApi(localApiClient);
builder.Services.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
builder.Services.AddSingleton<IFormMaker, FormMaker>();
builder.Services.AddSingleton<ILocalApi, LocalApi>();
builder.Services.AddSingleton<IMenuMaker, MenuMaker>();
builder.Services.AddSingleton<IMarkDownService, MarkDownService>();
var host =  builder.Build();

var markDownService = host.Services.GetRequiredService<IMarkDownService>();
markDownService.Init();

await host.RunAsync();
