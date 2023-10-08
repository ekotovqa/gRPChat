using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using gRPChat.Protos;
using gRPChat.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(services =>
{
    var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
    ChannelBase channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
    {
        HttpHandler = new GrpcWebHandler(new HttpClientHandler())
    });
    return new ChatRoom.ChatRoomClient(channel);
});

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var defaultCulture = new CultureInfo("en-Us");

var host = builder.Build();

var culture = await host.Services.GetRequiredService<ILocalStorageService>().GetItemAsStringAsync("lang_culture");

if (culture != null)
    defaultCulture = new CultureInfo(culture.Replace("\"", ""));

CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

await host.RunAsync();