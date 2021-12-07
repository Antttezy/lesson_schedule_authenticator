using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Core.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
