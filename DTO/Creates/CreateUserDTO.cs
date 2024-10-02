using System.ComponentModel.DataAnnotations;

namespace Trainning.DTO
{
    public record CreateUserDTO
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
        public string? Username { get; set; }

        public string? Phone { get; set; } = null!;
        public IFormFile? Avatar { get; set; } = null!;
        [Required]
        public IList<int> RoleIDs { get; set; } = [];
    }
}