namespace Trainning.DTO.Creates
{
    public record CreatePaymentDTO
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    
    }
}