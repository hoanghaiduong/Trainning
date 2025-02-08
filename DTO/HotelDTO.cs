

using Trainning.Entities;

namespace Trainning.DTO
{
    public class HotelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Thumbnail { get; set; }
        public List<string> Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        public int Stars { get; set; }
        public string? CheckinTime { get; set; } = DateTime.Now.ToString();
        public string? CheckoutTime { get; set; } = DateTime.Now.ToString();
          public virtual IList<User> Users { get; set; } = [];
        public virtual IList<Room> Rooms { get; set; } = [];
    }
}