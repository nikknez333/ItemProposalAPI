using FluentValidation;
using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ItemProposalAPI.Validation.Account
{
    public class RegisterAccountValidator : AbstractValidator<RegisterDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public RegisterAccountValidator(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            RuleFor(u => u.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(20).WithMessage("Username must not exceed 20 characters.")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("Username can only contain letters, numbers, and underscores.")
                .MustAsync(IsUsernameNotTaken).WithMessage("Username is taken, please provide another.");

            RuleFor(u => u.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one upperacase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must containt at least one special character.");
                
            RuleFor(u => u.PartyId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Party ID must be greater than 0.")
                .MustAsync(UserPartyExists).When(u => u.PartyId.HasValue).WithMessage(u => $"Party with iD: {u.PartyId} does not exist.");

        }

        private async Task<bool> UserPartyExists(int? partyId, CancellationToken token)
        {
            return await _unitOfWork.PartyRepository.ExistsAsync((int)partyId!);
        }

        private async Task<bool> IsUsernameNotTaken(string username, CancellationToken token)
        {
            return await _userManager.FindByNameAsync(username) == null;
        }
    }
}
