using System.Text.Json.Serialization;

namespace Trainning.Entities
{
    public class Role :BaseIdentity<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = null!;
        [JsonIgnore]
        public virtual IList<UserRoles> UserRoles { get;set; } = [];
    }
}