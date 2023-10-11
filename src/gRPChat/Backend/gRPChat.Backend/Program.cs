using gRPChat.Backend;
using gRPChat.Database;
using gRPChat.Database.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlite("Data Source=chat.db"));

builder.Services.AddIdentity<ChatUser, IdentityRole>()
    .AddEntityFrameworkStores<ChatDbContext>()
    .AddDefaultTokenProviders();

TokenParameters tokenParameters = new();

builder.Services.AddSingleton(tokenParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SecurityTokenValidators.Add(new ChatJwtValidator(tokenParameters));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc- Accept-Encoding");
}));

builder.Services.AddAuthentication();

builder.Services.AddSingleton<ChatRoomManager>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

// Configure the HTTP request pipeline.
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<ChatDbContext>().Database.EnsureCreated();

app.MapGrpcService<ChatRoomService>().EnableGrpcWeb();
app.MapGrpcService<AccountService>().EnableGrpcWeb();
app.MapFallbackToFile("index.html");

app.Run();

public class TokenParameters
{
    public string Issure => "issure";
    public string Audience => "audience";
    public string SecretKey => "secretKeysecretKeysecretKey";
    public DateTime Expiry => DateTime.Now.AddDays(1);
}
