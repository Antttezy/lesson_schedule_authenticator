using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticationService.Services.Abstract
{
    public abstract class TokenGenerator
    {
        public virtual string GenerateToken(SecurityKey secretKey, string issuer, string audience, TimeSpan expirationTime, IEnumerable<Claim> claims = null, string algorithm = SecurityAlgorithms.HmacSha256)
        {
            JwtSecurityToken token = new(
                issuer: issuer,
                audience: audience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(expirationTime),
                signingCredentials: new SigningCredentials(secretKey, algorithm),
                claims: claims
                );

            JwtSecurityTokenHandler handler = new();
            return handler.WriteToken(token);
        }
    }
}
