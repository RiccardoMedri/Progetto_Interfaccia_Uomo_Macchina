using System.ComponentModel.DataAnnotations;

namespace Medri.Web.Features.Login
{
    public static class AuthModes
    {
        public const string Login = "login";
        public const string Register = "register";
    }

    public class AuthPageViewModel
    {
        public string Mode { get; set; } = AuthModes.Login;

        public LoginViewModel Login { get; set; } = new LoginViewModel();

        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Rimani connesso")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public System.Guid? PendingFavoritePropertyId { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Cognome")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "La password deve contenere almeno 6 caratteri")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono")]
        [Display(Name = "Conferma password")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }

        public System.Guid? PendingFavoritePropertyId { get; set; }
    }
}
