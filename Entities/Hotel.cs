
using System.Text.Json.Serialization;

namespace Trainning.Entities
{
    public class Hotel : BaseIdentity<int>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Thumbnail { get; set; }
        public List<string> Images { get; set; } // Có thể là một chuỗi JSON hoặc đường dẫn đến hình ảnh
        public int Stars { get; set; }
        public string? CheckinTime { get; set; } = DateTime.Now.ToString();
        public string? CheckoutTime { get; set; } = DateTime.Now.ToString();
        [JsonIgnore]
        public virtual IList<User> Users { get; set; } = [];
        [JsonIgnore]
        public virtual IList<Room> Rooms { get; set; } = [];
    }

}