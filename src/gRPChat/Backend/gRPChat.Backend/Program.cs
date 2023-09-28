using gRPChat.Backend;
using gRPChat.Backend.Services;
using gRPChat.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlite("Data Source=chat.db"));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Configure the HTTP request pipeline.
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<ChatDbContext>().Database.EnsureCreated();

app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapGrpcService<ChatRoomService>().EnableGrpcWeb();
app.MapFallbackToFile("index.html");

app.Run();
