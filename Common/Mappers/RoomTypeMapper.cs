

using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class RoomTypeMapper
    {
        public static RoomTypeDTO ToRoomTypeDTO(this RoomType roomType)
        {
            return new RoomTypeDTO{
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                PricePerNight = roomType.PricePerNight,
                Capacity = roomType.Capacity,
                Rooms=roomType.Rooms,
            };
        }
    }
}