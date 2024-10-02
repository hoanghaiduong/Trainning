

using System.Text.Json.Serialization;

namespace Trainning.Entities
{
    public class RoomType : BaseIdentity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        [JsonIgnore]
        public virtual IList<Room> Rooms { get; set; } = [];
    }
}