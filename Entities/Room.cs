using System.Text.Json.Serialization;
using Trainning.Common.Enums;

namespace Trainning.Entities
{
    public class Room : BaseIdentity<int>
    {
        public string RoomNumber { get; set; }
        public string Thumbnail { get; set; } =null!;
        public List<string> Images { get; set; } =[];// Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        [JsonIgnore]
        public int HotelID { get; set; }
        [JsonIgnore]
        public int RoomTypeId { get; set; }
        public EStatusRoom Status { get; set; } = EStatusRoom.AVAILABLE;
        [JsonIgnore]
        public virtual RoomType RoomType { get; set; } = null!;
        [JsonIgnore]
        public virtual Hotel Hotel { get; set; } = null!;
    }


}