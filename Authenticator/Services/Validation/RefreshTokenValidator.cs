using AuthenticationService.Services.Access;
using AuthenticationService.Services.Refresh;
using System;

namespace AuthenticationService.Services.Validation
{
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
        private readonly JwtAccessTokenOptions options;
        private readonly JwtRefreshTokenOptions refreshOptions;

        public RefreshTokenValidator(JwtAccessTokenOptions options, JwtRefreshTokenOptions refreshOptions)
        {
            this.options = options;
            this.refreshOptions = refreshOptions;
        }

        public bool IsValid(string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new();

                handler.ValidateToken(token, new()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = refreshOptions.GetSymmetricSecurityKey()
                },
                out _);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
