using AuthenticationService.Models;

namespace AuthenticationService.Data
{
    public class AuthenticationContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<JwtToken> Tokens { get; set; }
        public AuthenticationContext(DbContextOptions options) : base(options)
        {

        }
    }
}
