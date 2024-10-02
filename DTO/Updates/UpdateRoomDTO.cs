

using System.ComponentModel.DataAnnotations;
using Trainning.Common.Enums;

namespace Trainning.DTO.Updates
{
    public record UpdateRoomDTO
    {
        public int? HotelID { get; set; }
        public int? RoomTypeId { get; set; }
        public string? RoomNumber { get; set; }
        public EStatusRoom Status = EStatusRoom.AVAILABLE;
        public IFormFile? Thumbnail { get; set; }
        public List<IFormFile>? Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        [Required]
        public string ExistsThumbnail { get; set; }
        [Required]
        public List<string> ExistsImages { get; set; } = [];

    }
}