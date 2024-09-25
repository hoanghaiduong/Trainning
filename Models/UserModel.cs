using System.Text.Json.Serialization;

namespace Trainning.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public List<string> Roles { get; set; } = [];
    }
}