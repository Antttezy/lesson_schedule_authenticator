using AuthenticationService.Models;
using AuthenticationService.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthenticationService.Services.Access
{
    public class JWTAccessTokenGenerator : TokenGenerator, IAccessTokenGenerator
    {
        private readonly JwtAccessTokenOptions options;

        public JWTAccessTokenGenerator(JwtAccessTokenOptions options)
        {
            this.options = options;
        }

        public string GenerateToken(Person person)
        {
            List<Claim> claims = new()
            {
                new(ClaimTypes.Name, person.Username),
                new(ClaimTypes.Role, person.Role.ToString()),
                new(nameof(person.Id), person.Id.ToString())
            };

            return GenerateToken(
                options.GetAsymmetricSecurityKey(),
                options.Issuer,
                options.Audience,
                TimeSpan.FromMinutes(options.Lifetime),
                claims,
                SecurityAlgorithms.RsaSha256);
        }
    }
}
