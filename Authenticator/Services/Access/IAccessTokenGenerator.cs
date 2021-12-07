using AuthenticationService.Models;

namespace AuthenticationService.Services.Access
{
    public interface IAccessTokenGenerator
    {
        string GenerateToken(Person person);
    }
}
