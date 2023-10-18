using Blazored.LocalStorage;
using gRPChat.Protos;
using gRPChat.Web.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly Account.AccountClient _accountClient;

    public IdentityAuthenticationStateProvider(ILocalStorageService localStorage, Account.AccountClient accountClient)
    {
        _localStorage = localStorage;
        _accountClient = accountClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");

        return await IsTokenValid(token)
            ? Jwt.GetStateFromJwt(token)
            : Empty();
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var authState = Task.FromResult(Jwt.GetStateFromJwt(token));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkLogouted()
    {
        await _localStorage.RemoveItemAsync("token");
        NotifyAuthenticationStateChanged(Task.FromResult(Empty()));
    }

    private async Task<bool> IsTokenValid(string token)
    {
        if (string.IsNullOrEmpty(token)) 
            return false;

        try
        {
            var authUser = await _accountClient.TokenValidateAsync(new UserInfoRequest());

            if (authUser.ResultCase == UserInfoResponse.ResultOneofCase.Profile)
                return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    private static AuthenticationState Empty() => new(new ClaimsPrincipal(new ClaimsIdentity()));
}