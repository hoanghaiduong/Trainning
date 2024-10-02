namespace Trainning.DTO
{
    public record CreateHotelDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IFormFile Thumbnail { get; set; }
        public List<IFormFile> Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        public int Stars { get; set; }
        public string? CheckinTime { get; set; }=DateTime.Now.ToString();
        public string? CheckoutTime { get; set; }=DateTime.Now.ToString();

    }
}