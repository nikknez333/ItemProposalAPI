using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ItemProposalAPI.Validation.Proposal
{
    public class AddCounterProposalValidator : AbstractValidator<CreateCounterProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AddCounterProposalValidator(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            RuleFor(cp => cp)
               .Cascade(CascadeMode.Stop)
               .CustomAsync(async (dto, validationContext, token) =>
               {
                   var proposalId = validationContext.RootContextData["ProposalId"] as int? ?? 0;
                   var username = validationContext.RootContextData["Username"] as string ?? "";

                   var user = await _userManager.FindByNameAsync(username);
                   validationContext.RootContextData["UserId"] = user.Id;

                   var userPartyId = user.PartyId;
                   validationContext.RootContextData["UserPartyId"] = userPartyId;
                   var ProposalToCounter = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId);

                   validationContext.RootContextData["ProposalToCounter"] = ProposalToCounter;

                   if (ProposalToCounter == null)
                   {
                       validationContext.AddFailure("ProposalId", $"Can't counter a proposal with ID:{proposalId}, because it doesn't exist.");
                       return;
                   }

                   if (ProposalToCounter.UserId == user.Id)
                   {
                       validationContext.AddFailure("UserId", "You cannot counter your own proposal.");
                       return;
                   }

                   var partiesSharingItem = await _unitOfWork.ItemPartyRepository
                       .QueryItemParty()
                       .Where(i => i.ItemId == ProposalToCounter.ItemId)
                       .Select(p => p.Party)
                       .ToListAsync();

                   var partiesSharingItemId = partiesSharingItem.Select(p => p.Id).ToList();

                   if (!partiesSharingItemId.Contains((int)user.PartyId))
                   {
                       validationContext.AddFailure("User.PartyId", $"User with ID:{user.UserName} cannot create proposal since item with id:{ProposalToCounter.ItemId} is not owned by user party.");
                       return;
                   }
               });

            RuleFor(cp => cp.Comment)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Comment is required")
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(cp => cp.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Payment ratios must be provided.")
                .WithMessage("Payment ratios for all involved parties must be included in the request.")
                .MustAsync((dto, paymentRatios, valContext, token) => AllInvolvedPartiesHavePaymentRatios(dto, paymentRatios, valContext, token))
                .WithMessage("Payment ratios for all involved parties must be included.")
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must equal 100 %.");
                /*.MustAsync(IsNewSetOfPaymentRatios).WithMessage("Your proposed set of payment ratios already exist, please provide new different set of payment ratios")*/
                

            RuleForEach(p => p.PaymentRatios).ChildRules(pr =>
            {
                pr.RuleFor(pr => pr.PartyId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Party ID is required.")
                    .GreaterThan(0).WithMessage("Party ID must be greater than 0.");

                pr.RuleFor(pr => pr.PaymentType)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Payment type is required.")
                    .IsInEnum().WithMessage("Invalid payment type. Allowed values: Fixed, Percentage.");

                pr.RuleFor(pr => pr.PaymentAmount)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Payment amount is required.")
                    .GreaterThanOrEqualTo(0).WithMessage("Payment amount must be greater than 0.");
            });
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

        /*private async Task<bool> IsNewSetOfPaymentRatios(CreateCounterProposalRequestDto dto, List<PaymentRatioDto> paymentRatios,
            ValidationContext<CreateCounterProposalRequestDto> valContext, CancellationToken token)
        {
            var proposalToCounterId = valContext.RootContextData["ProposalToCounter"] as int? ?? 0;

            var proposalToCounter = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalToCounterId);

            foreach(var proposalPaymentRatio in proposalToCounter.ProposalItemParties)
            {
                if(proposalPaymentRatio.PaymentAmount )
            }

        }*/

        private async Task<bool> AllInvolvedPartiesHavePaymentRatios(CreateCounterProposalRequestDto dto, List<PaymentRatioDto> paymentRatios, 
            ValidationContext<CreateCounterProposalRequestDto> valContext,
            CancellationToken token)
        {

            if (!valContext.RootContextData.TryGetValue("ProposalToCounter", out var originalProposalObj) || originalProposalObj is not Models.Proposal originalProposal)
            {
                return false;
            }

            var involvedParties = await _unitOfWork.ItemPartyRepository
                .QueryItemParty()
                .Where(i => i.ItemId == originalProposal.ItemId)
                .Select(p => p.Party)
                .ToListAsync();

            var involvedPartyIds = involvedParties.Select(p => p.Id).ToList();
            var providedPartyIds = paymentRatios.Select(pr => pr.PartyId ?? -1).ToList();

            return providedPartyIds.Count == involvedPartyIds.Count && !involvedPartyIds.Except(providedPartyIds).Any();
        }
    }
}
