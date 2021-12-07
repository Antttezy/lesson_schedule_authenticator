using AuthenticationService.Services.Abstract;
using AuthenticationService.Services.Access;
using System;

namespace AuthenticationService.Services.Refresh
{
    public class JWTRefreshTokenGenerator : TokenGenerator, IRefreshTokenGenerator
    {
        private readonly JwtRefreshTokenOptions refreshOptions;
        private readonly JwtAccessTokenOptions jwtOptions;

        public JWTRefreshTokenGenerator(JwtRefreshTokenOptions refreshOptions, JwtAccessTokenOptions jwtOptions)
        {
            this.refreshOptions = refreshOptions;
            this.jwtOptions = jwtOptions;
        }

        public string GenerateToken()
        {
            return GenerateToken(
                refreshOptions.GetSymmetricSecurityKey(),
                jwtOptions.Issuer,
                jwtOptions.Audience,
                TimeSpan.FromDays(refreshOptions.Lifetime));
        }
    }
}
