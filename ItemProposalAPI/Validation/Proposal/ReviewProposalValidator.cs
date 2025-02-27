using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ItemProposalAPI.Validation.Proposal
{
    public class ReviewProposalValidator : AbstractValidator<ReviewProposalDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public ReviewProposalValidator(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            RuleFor(ep => ep)
            .CustomAsync(async (dto, valContext, token) => 
                {
                    var username = valContext.RootContextData["Username"] as string;
                    var user = await _userManager.FindByNameAsync(username);
                    valContext.RootContextData["User"] = user;
                    var proposal = valContext.RootContextData["ProposalToEvaluate"] as ItemProposalAPI.Models.Proposal;

                    if (!proposal.Proposal_Status.Equals(Proposal_Status.Pending))
                    {
                        valContext.AddFailure("Proposal_Status", $"You cannot accept or reject a proposal with ID:{proposal.Id}, since it is already: {proposal.Proposal_Status}");
                        return;
                    }

                    if (proposal.UserId == user.Id)
                    {
                        if (dto.Response == Proposal_Status.Accepted)
                        {
                            valContext.AddFailure("ProposalId","You cannot accept your own proposal.");
                            return;
                        }
                        else if(dto.Response == Proposal_Status.Rejected)
                        {
                            valContext.AddFailure("ProposalId","You cannot reject your own proposal.");
                            return;
                        }
                    }

                    var userParty = user.PartyId;

                    var includedInDeal = proposal.ProposalItemParties.Any(pip => pip.ProposalId == proposal.Id && pip.ItemId == proposal.ItemId && pip.PartyId == userParty);
                    if (!includedInDeal)
                    {
                        valContext.AddFailure("PartyId", "You can't accept or reject a proposal since your party is not included in deal.");
                        return;
                    }
                });

            RuleFor(ep => ep.Response)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Response is required.")
                .IsInEnum().WithMessage("Invalid response type. Allowed valus: Accepted, Rejected");

            RuleFor(ep => ep.Comment)
                .NotEmpty()
                .When(ep => ep.Response == Proposal_Status.Rejected).WithMessage("Comment is required")
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(ep => ep.Comment)
                .Empty()
                .When(ep => ep.Response == Proposal_Status.Accepted).WithMessage("Comment is not allowed when accepting the proposal.")
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(ep => ep.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Payment ratios must be provided.")
                .CustomAsync(async (paymentRatios, valContext, token) =>
                {
                    var proposal = valContext.RootContextData["ProposalToEvaluate"] as ItemProposalAPI.Models.Proposal;

                    var involvedParties = await _unitOfWork.ItemPartyRepository
                        .QueryItemParty()
                        .Where(i => i.ItemId == proposal.ItemId)
                        .Select(p => p.Party)
                        .ToListAsync();

                    var involedPartyIds = involvedParties.Select(p => p.Id).ToList();
                    var providedPartyIds = paymentRatios.Select(pr => pr.PartyId ?? -1).ToList();

                    if(!(providedPartyIds.Count == involedPartyIds.Count && !involedPartyIds.Except(providedPartyIds).Any()))
                    {
                        valContext.AddFailure("Payment ratios for all and only all involved parties must be included in the request.");
                        return;
                    }
                })
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must be equal to 100 %.")
                .When(ep => ep.Response == Proposal_Status.Rejected);

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
            })
            .When(ep => ep.Response == Proposal_Status.Rejected);
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
