using FluentValidation;
using ItemProposalAPI.DTOs.Item;

namespace ItemProposalAPI.Validation.Item
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemRequestDto>
    {
        public UpdateItemValidator()
        {
            RuleFor(i => i.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(30).WithMessage("Item name maximum length is 30 characters.");
        }
    }
}
