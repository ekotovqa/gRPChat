using gRPChat.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace gRPChat.Backend
{
    public static class JwtTokenExtensions
    {
        public static async Task<string> GenerateJwtToken(this ChatUser user, TokenParameters tokenParameters, RoleManager<IdentityRole> roleManager, UserManager<ChatUser> userManager)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

                new(ClaimsIdentity.DefaultNameClaimType, user.UserName),

                new(ClaimTypes.NameIdentifier, user.Id),
            };

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParameters.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                tokenParameters.Issure,
                tokenParameters.Audience,
                claims,
                expires: tokenParameters.Expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
