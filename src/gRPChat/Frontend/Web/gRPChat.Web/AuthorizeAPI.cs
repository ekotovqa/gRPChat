using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace gRPChat.Web
{
    public class AuthorizeAPI
    {
        private readonly IdentityAuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthorizeAPI(AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _authenticationStateProvider = (IdentityAuthenticationStateProvider)authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<bool> Login(string login, string password)
        {
            var token = "123456789";


            await _localStorage.SetItemAsync("token", token);

            _authenticationStateProvider.MarkUserAsAuthenticated(token);

            return true;
        }
    }
}
