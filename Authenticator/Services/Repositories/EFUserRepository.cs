using AuthenticationService.Data;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthenticationService.Services.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AuthenticationContext authenticationContext;

        public EFUserRepository(AuthenticationContext authenticationContext)
        {
            this.authenticationContext = authenticationContext;
        }
        public async Task<Person> Add(Person person)
        {
            await authenticationContext.People.AddAsync(person);
            await authenticationContext.SaveChangesAsync();
            return person;
        }

        public async Task<Person> FindByName(string username)
        {
            return await authenticationContext.People.FirstOrDefaultAsync(p => p.Username == username);
        }

        public async Task<Person> Update(Person person)
        {
            Person toUpdate = await authenticationContext.People.FirstOrDefaultAsync(p => p == person);
            toUpdate.Username = person.Username;
            toUpdate.Password = person.Password;
            await authenticationContext.SaveChangesAsync();
            return toUpdate;
        }
    }
}
