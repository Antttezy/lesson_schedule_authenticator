using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Core.Requests
{
    public class LogoutRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
