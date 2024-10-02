
using System.Text.Json.Serialization;
using Trainning.Entities;

namespace Trainning.DTO
{
    public class BookingDTO
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public int RoomId { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual Room Room { get; set; } = null!;

        public virtual IList<Payment> Payments { get;set; }
    }
}