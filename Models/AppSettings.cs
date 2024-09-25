namespace Trainning.Models
{
    public class AppSettings
    {
        public string? Issuer { get; set; }  // The JWT Issuer (iss)
        public string? Audience { get; set; }  // The JWT Audience (aud)
        public string? AccessTokenSecret { get; set; } // The JWT
        public string? RefreshTokenSecret { get; set; } // The JWT Refresh
    }
}