

using System.Text.Json.Serialization;
using Trainning.Entities;

namespace Trainning.DTO
{
    public record UserDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; } = null!;
        public required string Email { get; set; }
        public bool? EmailVerified { get; set; } = false;
        public string? Phone { get; set; } = null!;
        public string? Avatar { get; set; } = null!;
        [JsonIgnore]
        public string? RefreshToken { get; set; } = null!;
        public bool IsDisabled { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public List<Role> Roles { get; set; } = null!;
        public virtual IList<Booking> Bookings { get; set; } = [];
    }
}