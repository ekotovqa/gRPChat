using Blazored.LocalStorage;
using gRPChat.Protos;
using Microsoft.AspNetCore.Components.Authorization;

namespace gRPChat.Web
{
    public class AuthorizeAPI
    {
        private readonly ILocalStorageService _localStorage;
        private readonly Account.AccountClient _accountClient;
        private readonly IdentityAuthenticationStateProvider _authenticationStateProvider;
        

        public AuthorizeAPI(AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage, Account.AccountClient accountClient)
        {
            _authenticationStateProvider = (IdentityAuthenticationStateProvider) authenticationStateProvider;
            _localStorage = localStorage;
            _accountClient = accountClient;
        }

        public async Task<bool> Login(string login, string password)
        {
            try
            {
                var tokenResponse = await _accountClient.LoginAsync(new LoginRequest
                {
                    Login = login,
                    Password = password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;
                    Console.WriteLine(token);
                    await _localStorage.SetItemAsync("token", token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(token);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public async Task<bool> Register(string userName, string password)
        {
            try
            {
                var tokenResponse = await _accountClient.RegisterAsync(new RegisterRequest
                {
                    Login = userName,
                    Password = password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;
                    Console.WriteLine(token);
                    await _localStorage.SetItemAsync("token", token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(token);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }       
    }
}
