using AuthenticationService.Models;
using System.Threading.Tasks;

namespace AuthenticationService.Services.Repositories
{
    public interface ITokenRepository
    {
        Task<JwtToken> Find(string token);
        Task<JwtToken> Add(JwtToken token);
        Task<JwtToken> Add(string token, Person owner);
        Task Remove(JwtToken token);
        Task RemoveByOwner(Person person);
    }
}
