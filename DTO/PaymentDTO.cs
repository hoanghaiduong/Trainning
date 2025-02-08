
using System.Text.Json.Serialization;
using Trainning.Common.Enums;
using Trainning.Entities;

namespace Trainning.DTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public EPaymentMethod PaymentMethod { get; set; } = EPaymentMethod.Cash;
        public virtual Booking Booking { get; set; } = null!;
    }
}