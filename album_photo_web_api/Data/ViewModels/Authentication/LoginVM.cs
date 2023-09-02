using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
