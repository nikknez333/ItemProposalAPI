using FluentValidation;
using ItemProposalAPI.DTOs.Account;

namespace ItemProposalAPI.Validation.Account
{
    public class LoginAccountValidator : AbstractValidator<LoginDto>
    {
        public LoginAccountValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Username is required");


            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
