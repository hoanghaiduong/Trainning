
using System.Text.Json.Serialization;

namespace Trainning.Entities
{
    public class User : BaseIdentity<int>
    {
        [JsonIgnore]
        public int? HotelId { get; set; } = null!;
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        public string? Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool? EmailVerified { get; set; } = false;
        public string? Phone { get; set; } = null!;
        public string? Avatar { get; set; } = null!;
        [JsonIgnore]
        public string? RefreshToken { get; set; } = null!;
        public bool IsDisabled { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        [JsonIgnore]
        public virtual Hotel? Hotel { get; set; } = null!;
        [JsonIgnore]
        public virtual IList<UserRoles> UserRoles { get; set; } = [];
        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set; } = [];
    }

}