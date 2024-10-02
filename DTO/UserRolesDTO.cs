using System.Text.Json.Serialization;
using Trainning.Entities;

namespace Trainning.DTO
{
    public class UserRolesDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public int RoleId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; } = null!;

        public virtual Role Role { get; set; } = null!;
    }
}