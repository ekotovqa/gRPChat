using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace gRPChat.Web.Helpers
{
    internal static class Extensions
    {
        public static async Task UseLocalization(this WebAssemblyHost host)
        {
            var defaultCulture = new CultureInfo("en-Us");

            var localStorage = host.Services.GetRequiredService<ILocalStorageService>();

            if (localStorage != null)
            {
                var culture = await localStorage.GetItemAsStringAsync("lang_culture");

                if (culture != null)
                    defaultCulture = new CultureInfo(culture.Replace("\"", ""));
            }

            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
        }

        public static void AddAuthGrpcClient<T>(this IServiceCollection services) where T : ClientBase
        {
            services.AddScoped(async provder =>
            {
                var client = (T)Activator.CreateInstance(typeof(T), await GetChanel(provder));
                return client;
            });
        }

        private static async Task<GrpcChannel> GetChanel(IServiceProvider provider)
        {
            var nav = provider.GetService<NavigationManager>();
            var storage = provider.GetService<ILocalStorageService>();

            string token = await storage.GetItemAsStringAsync("token");

            return nav.GetAuthChanel(token);
        }

        private static GrpcChannel GetAuthChanel(this NavigationManager navigation, string token) =>
            GrpcChannel.ForAddress(navigation.BaseUri, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())),
                Credentials = ChannelCredentials.Create(new SslCredentials(), GetJwtCredentials(token))
            });

        private static GrpcChannel GetAnonChanel(this NavigationManager navigation) =>
            GrpcChannel.ForAddress(navigation.BaseUri, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            });

        private static CallCredentials GetJwtCredentials(string token) =>
            CallCredentials.FromInterceptor((_, metadate) =>
            {
                metadate.AddJwt(token);
                return Task.CompletedTask;
            });

        private static void AddJwt(this Metadata metadata, string token)
        {
            if (!string.IsNullOrEmpty(token))
                metadata.Add("Authorization", $"Bearer {token}");
        }
    }
}
