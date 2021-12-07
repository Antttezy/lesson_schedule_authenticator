namespace AuthenticationService.Services.Validation
{
    public interface IRefreshTokenValidator
    {
        bool IsValid(string token);
    }
}
