using FluentValidation;
using ItemProposalAPI.DTOs.Party;

namespace ItemProposalAPI.Validation.Party
{
    public class UpdatePartyValidator : AbstractValidator<UpdatePartyRequestDto>
    {
        public UpdatePartyValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(30).WithMessage("Party name maximum length is 30 characters.");
        }
    }
}
