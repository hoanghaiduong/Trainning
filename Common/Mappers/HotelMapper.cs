

using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class HotelMapper
    {
        public static HotelDTO ToHotelDTO(this Hotel hotel){
            return new HotelDTO{
                Id=hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                Phone = hotel.Phone,
                Email= hotel.Email,
                Thumbnail = hotel.Thumbnail,
                Images = hotel.Images,
                Stars = hotel.Stars,
                CheckinTime=hotel.CheckinTime,
                CheckoutTime=hotel.CheckoutTime,
                Rooms=hotel.Rooms
            };
        }
    }
}