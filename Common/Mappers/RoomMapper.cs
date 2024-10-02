

using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class RoomMapper
    {
        public static RoomDTO ToRoomDTO(this Room room)
        {
            return new RoomDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Thumbnail = room.Thumbnail,
                Images = room.Images,
                Status=room.Status,
                Hotel=room.Hotel,
                RoomType = room.RoomType
            };
        }
    }
}