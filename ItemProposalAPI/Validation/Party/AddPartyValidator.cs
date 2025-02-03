using FluentValidation;
using ItemProposalAPI.DTOs.Party;

namespace ItemProposalAPI.Validation.Party
{
    public class AddPartyValidator : AbstractValidator<CreatePartyRequestDto>
    {
        public AddPartyValidator()
        { 
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(30).WithMessage("Party name maximum length is 30 characters.");
        }
    }
}
