
using Trainning.Entities;

namespace Trainning.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = null!;
      
        public virtual IList<UserRoles> UserRoles { get; set; } = [];
    }
}