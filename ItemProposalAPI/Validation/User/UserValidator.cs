using FluentValidation;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.UnitOfWorkPattern.UnitOfWork;

namespace ItemProposalAPI.Validation.User
{
    public class UserValidator : AbstractValidator<CreateUserRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(30).WithMessage("Username maximum length is 30.");

            RuleFor(u => u.PartyId)
                .GreaterThan(0).WithMessage("Party ID must be greater than 0.")
                .MustAsync(UserPartyExists).When(u => u.PartyId.HasValue).WithMessage(u => $"Party with iD: {u.PartyId} does not exist.");
        }

        private async Task<bool> UserPartyExists(int? partyId, CancellationToken token)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync((int)partyId!);

            return party != null;
        }
    }
}
