using AuthenticationService.Models;
using System.Threading.Tasks;

namespace AuthenticationService.Services.Repositories
{
    public interface IUserRepository
    {
        Task<Person> FindByName(string username);
        Task<Person> Add(Person person);
        Task<Person> Update(Person person);
    }
}
