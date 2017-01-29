using FluentValidation;
using HomeCinema.Web.Models;

namespace HomeCinema.Web.Infrastructure.Validators
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            this.RuleFor(r => r.Email).NotEmpty().EmailAddress()
                .WithMessage("Invalid email address");

            this.RuleFor(r => r.Username).NotEmpty()
                .WithMessage("Invalid username");

            this.RuleFor(r => r.Password).NotEmpty()
                .WithMessage("Invalid password");
        }
    }

    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            this.RuleFor(r => r.Username).NotEmpty()
                .WithMessage("Invalid username");

            this.RuleFor(r => r.Password).NotEmpty()
                .WithMessage("Invalid password");
        }
    }
}