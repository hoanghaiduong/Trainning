
using System.Text.Json.Serialization;
using Trainning.Common.Enums;
using Trainning.Entities;

namespace Trainning.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string? Thumbnail { get; set; }
        public List<string>? Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        public EStatusRoom Status { get; set; } = EStatusRoom.AVAILABLE;
        [JsonIgnore]
        public int HotelID { get; set; }
        [JsonIgnore]
        public int RoomTypeId { get; set; }
        public virtual Hotel Hotel { get; set; }=null!;
        public virtual RoomType RoomType { get; set; } =null!;
    }
}