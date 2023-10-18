using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace gRPChat.Backend
{
    public class ChatJwtValidator : ISecurityTokenValidator
    {
        private string _audience;
        private string _issure;
        private string _secretKey;

        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; } = int.MaxValue;

        public ChatJwtValidator(TokenParameters tokenParameters)
        {
            _audience = tokenParameters.Audience;
            _issure = tokenParameters.Issure;
            _secretKey = tokenParameters.SecretKey;
        }

        public bool CanReadToken(string securityToken) => true;

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _issure,
                ValidAudience = _audience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            };

            try
            {
                var claimsPrincipal = handler.ValidateToken(securityToken, tokenValidationParameters, out validatedToken);

                return claimsPrincipal;
            }
            catch (Exception)
            {
                validatedToken = new JwtSecurityToken();
                return new ClaimsPrincipal();
            }
        }

    }
}
