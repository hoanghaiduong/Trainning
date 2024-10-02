
using System.ComponentModel.DataAnnotations;

namespace Trainning.DTO
{
    public class AuthDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Username { get; set; }

    }
}