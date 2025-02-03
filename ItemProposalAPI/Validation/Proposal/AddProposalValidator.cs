using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ItemProposalAPI.Validation.Proposal
{
    public class AddProposalValidator : AbstractValidator<CreateProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddProposalValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User ID is required.")
                .GreaterThan(0).WithMessage("User ID must be higher than 0")
                .MustAsync(UserExists).WithMessage(p => $"User with ID:{p.UserId} does not exist.")
                .MustAsync(UserBelongsToParty).WithMessage(p => $"User with ID:{p.UserId} is not part of any party, only users that are associated with some party can create proposals.");

            RuleFor(p => p.ItemId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Item ID is required.")
                .GreaterThan(0).WithMessage("Item ID must be higher than 0")
                .MustAsync(ItemExists).WithMessage(p => $"Item with ID:{p.ItemId} does not exist.")
                .MustAsync(IsItemShared).WithMessage(p => $"Proposal for item with ID: {p.ItemId} can't be made, because the item is not shared.")
                .MustAsync(ItemHasNoExistingProposal).WithMessage(p => $"A proposal already exists for this Item with ID:{p.ItemId}. Please submit a counterproposal instead.");

            RuleFor(p => p.Comment)
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(p => p.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Payment ratios must be provided.")
                .MustAsync((dto, paymentRatios, token) => AllInvolvedPartiesHavePaymentRatios(dto.ItemId, paymentRatios, token))
                .WithMessage("Payment ratios for all involved parties must be included in the request.")
                .MustAsync(NoDuplicateParties).WithMessage($"Duplicate payment ratios found.")
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must be equal to 100 %.");

            RuleForEach(p => p.PaymentRatios).ChildRules(pr =>
            {
                pr.RuleFor(pr => pr.PartyId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Party ID is required.")
                    .GreaterThan(0).WithMessage("Party ID must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentAmount)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Payment amount is required.")
                    .GreaterThanOrEqualTo(0).WithMessage("Payment amount must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentType)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Payment type (Fixed or Percentage) is required.")
                    .IsInEnum().WithMessage("Invalid payment type.");
            });

        }

        private async Task<bool> UserExists(int userId, CancellationToken token)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            return user != null;
        }

        private async Task<bool> UserBelongsToParty(int userId, CancellationToken token)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            return user?.PartyId != null;
        }

        private async Task<bool> ItemExists(int itemId, CancellationToken token)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);

            return item != null;
        }

        private async Task<bool> IsItemShared(int itemId, CancellationToken token)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);

            return item?.Share_Status != Status.Not_Shared;
        }

        private async Task<bool> ItemHasNoExistingProposal(int itemId, CancellationToken token)
        {
            var existingProposal = await _unitOfWork.ProposalRepository.GetNegotiationDetails(itemId);

            return existingProposal != null || !existingProposal.Any();
        }

        private async Task<bool> AllInvolvedPartiesHavePaymentRatios(int itemId, List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            var involvedParties = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(itemId);
            var involedPartyIds = involvedParties.Select(p => p.Id).ToList();
            var providedPartyIds = paymentRatios.Select(pr => pr.PartyId).ToList();

            return providedPartyIds.Count == involedPartyIds.Count && !involedPartyIds.Except(providedPartyIds).Any();
        }

        private async Task<bool> NoDuplicateParties(List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            return paymentRatios.GroupBy(pip => pip.PartyId).All(g => g.Count() == 1);
        }

        private async Task<bool> HaveSamePaymentType(List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            return paymentRatios.Select(pr => pr.PaymentType).Distinct().Count() == 1;
        }

        private async Task<bool> IsValidPercentageTotal(List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            if (paymentRatios.First().PaymentType == PaymentType.Percentage)
            {
                return paymentRatios.Sum(pr => pr.PaymentAmount) == 100;
            }
            return true;
        }
    }
}
