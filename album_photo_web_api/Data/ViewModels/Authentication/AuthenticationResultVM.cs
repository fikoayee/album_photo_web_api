namespace album_photo_web_api.Data.ViewModels.Authentication
{
    public class AuthenticationResultVM
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
