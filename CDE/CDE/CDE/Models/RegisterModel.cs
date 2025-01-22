using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Please enter your email.")]
        [DomainEmailValidation]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        public string? Password { get; set; }
    }
    public class DomainEmailValidationAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain = "@domain.com";

        public DomainEmailValidationAttribute() : base("Email must have the domain @unilever.com")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is string email && !string.IsNullOrEmpty(email))
            {
                return email.EndsWith(_allowedDomain, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}

