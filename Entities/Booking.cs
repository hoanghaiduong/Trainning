

using System.Text.Json.Serialization;

namespace Trainning.Entities
{
    public class Booking : BaseIdentity<int>
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public decimal TotalPrice { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; } = null!;
        [JsonIgnore]
        public virtual Room Room { get; set; } = null!;
        [JsonIgnore]
        public virtual IList<Payment> Payments { get; }
    }

}