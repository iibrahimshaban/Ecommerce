using Ecommerce.Application.Common.Validation;

namespace Ecommerce.Application.Contracts.Auth;
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(RegexPattern.Email).WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .Matches(RegexPattern.Password)
            .WithMessage("Password must contain upper, lower, digit, special char, and be at least 8 chars.");

    }
}
