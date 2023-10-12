using Blazored.LocalStorage;
using gRPChat.Protos;
using gRPChat.Web;
using gRPChat.Web.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthGrpcClient<Account.AccountClient>();
builder.Services.AddAuthGrpcClient<ChatRoom.ChatRoomClient>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthorizeAPI>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var host = builder.Build();

await host.UseLocalization();

await host.RunAsync();