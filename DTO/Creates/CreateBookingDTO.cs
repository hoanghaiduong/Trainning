namespace Trainning.DTO.Creates
{
    public record CreateBookingDTO
    {
          public int RoomId { get; set; }
        public int UserId { get; set; }
      
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}