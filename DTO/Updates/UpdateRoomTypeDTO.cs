namespace Trainning.DTO.Updates
{
    public class UpdateRoomTypeDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
    }
}