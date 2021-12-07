using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models
{
    public class JwtToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public Person Owner { get; set; }
    }
}
