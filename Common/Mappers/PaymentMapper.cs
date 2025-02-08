
using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentDTO ToPaymentDTO(this Payment payment)
        {
            return new PaymentDTO
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Booking = payment.Booking,
            };
        }
    }
}