using Trainning.Entities;

namespace Trainning.DTO
{
    public class RoomTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public virtual IList<Room> Rooms { get; set; } = [];
    }
}