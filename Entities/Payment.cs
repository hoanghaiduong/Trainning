
using System.Text.Json.Serialization;
using Trainning.Common.Enums;

namespace Trainning.Entities
{
    public class Payment : BaseIdentity<int>
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public EPaymentMethod PaymentMethod { get; set; } = EPaymentMethod.Cash;
        [JsonIgnore]
        public virtual Booking Booking { get; set; }
    }

}