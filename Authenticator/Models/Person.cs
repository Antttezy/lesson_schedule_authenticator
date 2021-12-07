using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public PersonRole Role { get; set; }
    }
}
