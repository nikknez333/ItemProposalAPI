using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ItemProposalAPI.Validation.Proposal
{
    public class AddProposalValidator : AbstractValidator<CreateProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AddProposalValidator(IUnitOfWork unitOfWork, UserManager<User> userManager) 
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            RuleFor(p => p.ItemId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Item ID is required.")
                .GreaterThan(0).WithMessage("Item ID must be higher than 0")
                .MustAsync(ItemExists).WithMessage(p => $"Item with ID:{p.ItemId} does not exist.")
                .MustAsync(IsItemShared).WithMessage(p => $"Proposal for item with ID: {p.ItemId} can't be made, because the item is not shared.")
                .MustAsync(ItemHasNoExistingProposal).WithMessage(p => $"A proposal already exists for this Item with ID:{p.ItemId}. Please submit a counterproposal instead.")
                .CustomAsync(async (itemId, valContext, token) =>
                {
                    var username = valContext.RootContextData["Username"] as string ?? "";
                    var user = await _userManager.FindByNameAsync(username);

                    var userPartyId = user.PartyId;
                    valContext.RootContextData["UserPartyId"] = userPartyId;
                    if (userPartyId == null)
                        valContext.AddFailure("ItemId", $"Can't create proposal for item with ID:{itemId}, because user is not employed by any party.");

                    var partiesSharingItem = await _unitOfWork.ItemPartyRepository
                        .QueryItemParty()
                        .Where(i => i.ItemId == itemId)
                        .Select(p => p.Party)
                        .ToListAsync();

                    var partiesIds = partiesSharingItem.Select(p => p.Id).ToList();

                    if(!partiesIds.Contains((int)userPartyId!))
                        valContext.AddFailure("ItemId", $"Can't create proposal for item with ID:{itemId}, because item is not owned by your party with ID: {userPartyId}");
                });

            RuleFor(p => p.Comment)
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");

            RuleFor(p => p.PaymentRatios)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Payment ratios must be provided.")
                .MustAsync((dto, paymentRatios, token) => AllInvolvedPartiesHavePaymentRatios(dto.ItemId, paymentRatios, token))
                .WithMessage("Payment ratios for all and only all involved parties must be included in the request.")
                .MustAsync(HaveSamePaymentType).WithMessage("All payment ratios in a proposal must have the same payment type(either all Fixed or all Percentage).")
                .MustAsync(IsValidPercentageTotal).WithMessage("The total of all payment ratios(percentages) must be equal to 100 %.");

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
                    .GreaterThanOrEqualTo(0).WithMessage("Payment amount cannot be negative value.");
            });

        }

        private async Task<bool> ItemExists(int? itemId, CancellationToken token)
        {
            return await _unitOfWork.ItemRepository.ExistsAsync((int)itemId);
        }

        private async Task<bool> IsItemShared(int? itemId, CancellationToken token)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync((int)itemId);

            return item?.Share_Status != Status.Not_Shared;
        }

        private async Task<bool> ItemHasNoExistingProposal(int? itemId, CancellationToken token)
        {
            var existingProposal = await _unitOfWork.ProposalRepository.GetNegotiationDetails((int)itemId);

            return existingProposal != null && !existingProposal.Any();
        }

        private async Task<bool> AllInvolvedPartiesHavePaymentRatios(int? itemId, List<PaymentRatioDto>? paymentRatios, CancellationToken token)
        {
            var involvedParties = await _unitOfWork.ItemPartyRepository
                .QueryItemParty()
                .Where(i => i.ItemId == itemId)
                .Select(p => p.Party)
                .ToListAsync();

            var involedPartyIds = involvedParties.Select(p => p.Id).ToList();
            var providedPartyIds = paymentRatios.Select(pr => pr.PartyId ?? -1).ToList();

            return providedPartyIds.Count == involedPartyIds.Count && !involedPartyIds.Except(providedPartyIds).Any();
        }

        private async Task<bool> HaveSamePaymentType(List<PaymentRatioDto>? paymentRatios, CancellationToken token)
        {
            return paymentRatios.Select(pr => pr.PaymentType).Distinct().Count() == 1;
        }

        private async Task<bool> IsValidPercentageTotal(List<PaymentRatioDto>? paymentRatios, CancellationToken token)
        {
            if (paymentRatios.First().PaymentType == PaymentType.Percentage)
                return paymentRatios.Sum(pr => pr.PaymentAmount) == 100;
            
            return true;
        }
    }
}
