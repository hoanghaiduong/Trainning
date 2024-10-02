

namespace Trainning.DTO.Updates
{
    public record UpdateHotelDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Stars { get; set; }
        public IFormFile? Thumbnail { get; set; } = null!;
        public string ExistsThumbnail { get; set; }
        public List<string> ExistingImages { get; set; } // URLs of existing images
        public List<IFormFile>? Images { get; set; }     // New files to be uploaded
        public string? CheckinTime { get; set; } = DateTime.Now.ToString();
        public string? CheckoutTime { get; set; } = DateTime.Now.ToString();
    }

}