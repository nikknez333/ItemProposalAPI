using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.EntityFrameworkCore;

namespace ItemProposalAPI.Validation.Proposal
{
    public class UpdateProposalValidator : AbstractValidator<UpdateProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork; 
        public UpdateProposalValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(ep => ep.Comment)
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(ep => ep.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .CustomAsync(async (paymentRatios, valContext, token) =>
                {
                    var proposalId = valContext.RootContextData["ProposalId"] as int? ?? 0;

                    var proposal = valContext.RootContextData["ProposalToEvaluate"] as ItemProposalAPI.Models.Proposal;

                    valContext.RootContextData["ProposalToEvaluate"] = proposal;

                    var involvedParties = await _unitOfWork.ItemPartyRepository
                        .QueryItemParty()
                        .Where(i => i.ItemId == proposal.ItemId)
                        .Select(p => p.Party)
                        .ToListAsync();

                    var involedPartyIds = involvedParties.Select(p => p.Id).ToList();
                    var providedPartyIds = paymentRatios.Select(pr => pr.PartyId ?? -1).ToList();

                    if (!(providedPartyIds.Count == involedPartyIds.Count && !involedPartyIds.Except(providedPartyIds).Any()))
                    {
                        valContext.AddFailure("Payment ratios for all and only all involved parties must be included in the request.");
                        return;
                    }
                })
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must be equal to 100 %.");

            RuleForEach(ep => ep.PaymentRatios).ChildRules(pr =>
            {
                pr.RuleFor(pr => pr.PartyId)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("Party ID is required.")
                    .GreaterThan(0).WithMessage("Party ID must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentType)
                     .Cascade(CascadeMode.Stop)
                     .NotEmpty().WithMessage("Payment type is required.")
                     .IsInEnum().WithMessage("Invalid payment type. Allowed values: Fixed, Percentage.");

                pr.RuleFor(pr => pr.PaymentAmount)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("Payment amount is required.")
                    .GreaterThanOrEqualTo(0).WithMessage("Payment amount cannot be negative value.");
            });
        }

        private async Task<bool> HaveSamePaymentType(List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            return paymentRatios.Select(pr => pr.PaymentType).Distinct().Count() == 1;
        }

        private async Task<bool> IsValidPercentageTotal(List<PaymentRatioDto> paymentRatios, CancellationToken token)
        {
            if (paymentRatios.First().PaymentType == PaymentType.Percentage)
                return paymentRatios.Sum(pr => pr.PaymentAmount) == 100;

            return true;
        }
    }
}
