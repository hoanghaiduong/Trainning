using Trainning.Common.Enums;

namespace Trainning.DTO.Creates
{
    public record CreateRoomDTO
    {

        public required string RoomNumber { get; set; }
        public required IFormFile Thumbnail { get; set; }
        public required List<IFormFile> Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        public EStatusRoom Status = EStatusRoom.AVAILABLE;
        public int HotelID { get; set; }
        public int RoomTypeId { get; set; }
    }
}