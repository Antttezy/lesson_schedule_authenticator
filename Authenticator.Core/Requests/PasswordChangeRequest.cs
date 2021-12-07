using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Core.Requests
{
    public class PasswordChangeRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
