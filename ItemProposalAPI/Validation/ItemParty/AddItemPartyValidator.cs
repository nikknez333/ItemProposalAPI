using FluentValidation;
using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.UnitOfWorkPattern.Interface;

namespace ItemProposalAPI.Validation.ItemParty
{
    public class AddItemPartyValidator : AbstractValidator<CreateItemPartyRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddItemPartyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(ip => ip.PartyId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Party ID is required.")
                .GreaterThan(0).WithMessage("Party ID value must be greater than 0.")
                .MustAsync(PartyExists).WithMessage(ip => $"Party with ID: {ip.PartyId} does not exist.");

            RuleFor(ip => ip.ItemId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Item ID is required")
                .GreaterThan(0).WithMessage("Item ID value must be greater than 0.")
                .MustAsync(ItemExists).WithMessage(ip => $"Item with ID: {ip.ItemId} does not exist.");
        }

        private async Task<bool> PartyExists(int partyId, CancellationToken token)
        {
            return await _unitOfWork.PartyRepository.ExistsAsync(partyId);
        }

        private async Task<bool> ItemExists(int itemId, CancellationToken token)
        {
            return await _unitOfWork.ItemRepository.ExistsAsync(itemId);
        }
    }
}
