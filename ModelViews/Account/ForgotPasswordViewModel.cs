using System.ComponentModel.DataAnnotations;

namespace Injazat.Presentation.ModelViews.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}
