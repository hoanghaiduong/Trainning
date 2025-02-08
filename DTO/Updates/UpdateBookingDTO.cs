

namespace Trainning.DTO.Updates
{
    public record UpdateBookingDTO
    {
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public DateTime? CheckinDate { get; set; }
        public DateTime? CheckoutDate { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}