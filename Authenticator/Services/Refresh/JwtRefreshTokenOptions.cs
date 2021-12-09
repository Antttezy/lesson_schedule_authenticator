using Microsoft.Extensions.Configuration;
using System;

namespace AuthenticationService.Services.Refresh
{
    public class JwtRefreshTokenOptions
    {
        private readonly string key;

        public int Lifetime { get; }

        public JwtRefreshTokenOptions(IConfiguration configuration)
        {
            var refreshTokenOptions = configuration.GetSection("RefreshOptions");
            key = refreshTokenOptions.GetValue<string>("key");
            Lifetime = refreshTokenOptions.GetValue<int>("Lifetime");
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Convert.FromBase64String(key));
    }
}
