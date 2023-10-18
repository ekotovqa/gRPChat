using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace gRPChat.Web.Helpers
{
    internal static class Jwt
    {
        public static AuthenticationState GetStateFromJwt(string token)
           => new(new ClaimsPrincipal(GetIdentityFromJwtToken(token)));

        private static ClaimsIdentity GetIdentityFromJwtToken(string token)
            => new(ParseClaimsFromJwtToken(token), "jwt");

        private static IEnumerable<Claim> ParseClaimsFromJwtToken(string jwt)
            => new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims;
    }
}
