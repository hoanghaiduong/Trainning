
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Trainning.Entities
{
    public class User : BaseIdentity<int>
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
       
        public string? Username { get; set; }
        [JsonIgnore]
        public required string Password { get; set; }
        public required string Email { get; set; }
        public bool? EmailVerified { get; set; } = false;
        public string? Phone { get; set; } = null!;
        public string? Avatar { get; set; } = null!;
        public string? RefreshToken { get; set; } = null!;
        public bool IsDisabled { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        [JsonIgnore]
        public virtual IList<UserRoles> UserRoles { get;set; } = [];
        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set;} = [];
    }

}