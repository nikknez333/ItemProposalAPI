using FluentValidation;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.UnitOfWorkPattern.Interface;

namespace ItemProposalAPI.Validation.User
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(30).WithMessage("Username maximum length is 30.");

            RuleFor(u => u.PartyId)
                .GreaterThan(0).WithMessage("Party Id must be greater than 0.")
                .MustAsync(UserPartyExists).When(u => u.PartyId.HasValue).WithMessage(u => $"Party with id:{u.PartyId} does not exist.");
        }


        private async Task<bool> UserPartyExists(int? partyId, CancellationToken token)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync((int)partyId!);

            return party != null;
        }
    }
}
