

namespace Trainning.DTO.Updates
{
    public class UpdateUserDTO
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Username { get; set; }
        public string? Phone { get; set; } = null!;
        public IFormFile? Avatar { get; set; } = null!;
        public IList<int>? RoleIDs { get; set; } = [];
    }
}