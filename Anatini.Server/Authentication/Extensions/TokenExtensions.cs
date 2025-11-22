using System.IdentityModel.Tokens.Jwt;

namespace Anatini.Server.Authentication.Extensions
{
    public static class TokenExtensions
    {
        public static string? GetClaimValue(this string tokenString, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // TODO use ValidateToken and handle invalid tokens correctly
            var token = tokenHandler.ReadJwtToken(tokenString);

            return token.Claims.FirstOrDefault(a => a.Type == claimType)?.Value ?? null;
        }
    }
}
