using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using System.Linq;

namespace ItemProposalAPI.Validation.Proposal
{
    public class AddCounterProposalValidator : AbstractValidator<CreateCounterProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCounterProposalValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(cp => cp.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThan(0).WithMessage("User ID must be greater than 0.")
                .MustAsync(UserExists).WithMessage(cp => $"User with ID:{cp.UserId} does not exist.")
                .MustAsync(UserBelongsToParty).WithMessage(cp => $"User with ID:{cp.UserId} is not part of any party, therefore it is not possible to make proposal.");


            RuleFor(cp => cp.Comment)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Comment is required")
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(cp => cp)
                .CustomAsync(async (dto, validationContext, token) =>
                {
                    var proposalId = validationContext.RootContextData["ProposalId"] as int? ?? 0;
                    var originalProposal = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId);

                    if (originalProposal == null)
                    {
                        validationContext.AddFailure("ProposalId", "The original proposal does not exist.");
                        return;
                    }

                    if (originalProposal.UserId == dto.UserId)
                    {
                        validationContext.AddFailure("UserId", "You cannot counter your own proposal.");
                        return;
                    }

                    validationContext.RootContextData["OriginalProposal"] = originalProposal;
                });



            RuleFor(cp => cp.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Payment ratios must be provided.")
                .WithMessage("Payment ratios for all involved parties must be included in the request.")
                .MustAsync(NoDuplicateParties).WithMessage("Duplicate payment ratios found for a PartyId.")
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must equal 100 %.")
                .MustAsync((dto, paymentRatios, valContext, token) => AllInvolvedPartiesHavePaymentRatios(dto, paymentRatios, valContext, token))
                .WithMessage("Payment ratios for all involved parties must be included.");

            RuleForEach(p => p.PaymentRatios).ChildRules(pr =>
            {
                pr.RuleFor(pr => pr.PartyId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Party ID is required.")
                    .GreaterThan(0).WithMessage("Party ID must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentAmount)
                    .GreaterThanOrEqualTo(0).WithMessage("Payment amount must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentType)
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

        private async Task<bool> AllInvolvedPartiesHavePaymentRatios(CreateCounterProposalRequestDto dto, List<PaymentRatioDto> paymentRatios, 
            ValidationContext<CreateCounterProposalRequestDto> valContext,
            CancellationToken token)
        {

            if (!valContext.RootContextData.TryGetValue("OriginalProposal", out var originalProposalObj) || originalProposalObj is not Models.Proposal originalProposal)
            {
                return false;
            }

            var involvedParties = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(originalProposal.ItemId);
            var involvedPartyIds = involvedParties.Select(p => p.Id).ToList();
            var providedPartyIds = paymentRatios.Select(pr => pr.PartyId).ToList();

            return providedPartyIds.Count == involvedPartyIds.Count && !involvedPartyIds.Except(providedPartyIds).Any();
        }
    }
}
