using AuthenticationService.Data;
using AuthenticationService.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Services.Repositories
{
    public class EFTokenRepository : ITokenRepository
    {
        private readonly AuthenticationContext authenticationContext;

        public EFTokenRepository(AuthenticationContext authenticationContext)
        {
            this.authenticationContext = authenticationContext;
        }

        public async Task<JwtToken> Add(JwtToken token)
        {
            await authenticationContext.Tokens.AddAsync(token);
            await authenticationContext.SaveChangesAsync();
            return token;
        }

        public async Task<JwtToken> Add(string token, Person owner)
        {
            JwtToken jwt = new()
            {
                Owner = owner,
                Token = token
            };

            jwt = await Add(jwt);
            return jwt;
        }

        public async Task<JwtToken> Find(string token)
        {
            return await authenticationContext.Tokens.Include(t => t.Owner).FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task Remove(JwtToken token)
        {
            authenticationContext.Tokens.Remove(token);
            await authenticationContext.SaveChangesAsync();
        }

        public async Task RemoveByOwner(Person person)
        {
            IQueryable<JwtToken> tokensToRemove = authenticationContext.Tokens
                .Include(t => t.Owner)
                .Where(t => t.Owner == person);

            authenticationContext.RemoveRange(tokensToRemove);
            await authenticationContext.SaveChangesAsync();
        }
    }
}
