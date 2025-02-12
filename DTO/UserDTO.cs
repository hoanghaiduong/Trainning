

using System.Text.Json.Serialization;
using Trainning.Entities;

namespace Trainning.DTO
{
    public record UserDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; } = null!;
        public required string Email { get; set; }
        public bool? EmailVerified { get; set; } = false;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public bool IsDisabled { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public virtual Hotel? Hotel { get; set; } 
        public virtual IList<Role> Roles { get; set; } = [];
        public virtual IList<Booking> Bookings { get; set; } = [];
    }
}