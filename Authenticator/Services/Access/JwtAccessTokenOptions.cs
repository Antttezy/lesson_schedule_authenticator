using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace AuthenticationService.Services.Access
{
    public class JwtAccessTokenOptions
    {
        private readonly string key;

        private readonly string privateKey;
        private readonly string publicKey;

        public string Issuer { get; }
        public string Audience { get; }
        public int Lifetime { get; }

        public JwtAccessTokenOptions(IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JWTOptions");

            key = jwtOptions.GetValue<string>("key");
            privateKey = jwtOptions.GetValue<string>("private_key");
            publicKey = jwtOptions.GetValue<string>("public_key");
            Issuer = jwtOptions.GetValue<string>("Issuer");
            Audience = jwtOptions.GetValue<string>("Audience");
            Lifetime = jwtOptions.GetValue<int>("Lifetime");
        }

        [Obsolete("Using symmetric keys for access JWT may be unsafe, use asymmetric key pair instead")]
        public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Convert.FromBase64String(key));

        public AsymmetricSecurityKey GetAsymmetricSecurityKey()
        {
            var rsa = RSA.Create();

            if (!string.IsNullOrEmpty(privateKey))
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            }
            else
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            }

            return new RsaSecurityKey(rsa);
        }
    }
}
