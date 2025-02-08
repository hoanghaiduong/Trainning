namespace Trainning.DTO.Updates
{
    public record UpdatePaymentDTO
    {
        public int? BookingId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
       
    }
}