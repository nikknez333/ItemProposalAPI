using FluentValidation;
using ItemProposalAPI.DTOs.Item;

namespace ItemProposalAPI.Validation.Item
{
    public class AddItemValidator : AbstractValidator<CreateItemRequestDto>
    {
        public AddItemValidator()
        {
            RuleFor(i => i.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(30).WithMessage("Item name maximum length is 30 characters.");
        }
    }
}
