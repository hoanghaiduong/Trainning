
using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class BookingMapper
    {
        public static BookingDTO ToBookingDTO(this Booking booking)
        {
            return new BookingDTO
            {
                Id = booking.Id,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                CheckinDate = booking.CheckinDate,
                CheckoutDate = booking.CheckoutDate,
                TotalPrice = booking.TotalPrice,
                User = booking.User,
                Room = booking.Room,
                Payments = booking.Payments
            };
        }
    }
}