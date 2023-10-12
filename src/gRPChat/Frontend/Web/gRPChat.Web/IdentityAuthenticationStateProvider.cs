using Blazored.LocalStorage;
using gRPChat.Protos;
using gRPChat.Web.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly Task<Account.AccountClient> _accountClient;

    public IdentityAuthenticationStateProvider(ILocalStorageService localStorage, Task<Account.AccountClient> accountClient)
    {
        _localStorage = localStorage;
        _accountClient = accountClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("token");

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var authUser = await (await _accountClient).TokenValidateAsync(new UserInfoRequest());

                if (authUser.ResultCase == UserInfoResponse.ResultOneofCase.Profile)
                    return Jwt.GetStateFromJwt(token);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        return new(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var authState = Task.FromResult(Jwt.GetStateFromJwt(token));

        NotifyAuthenticationStateChanged(authState);
    }
}