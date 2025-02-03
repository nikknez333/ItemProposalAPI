using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.UnitOfWorkPattern.Interface;

namespace ItemProposalAPI.Validation.Proposal
{
    public class UpdateProposalValidator : AbstractValidator<UpdateProposalRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork; 
        public UpdateProposalValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.Comment)
                .MaximumLength(100).WithMessage("Comment maximum length is 100.");
        }
    }
}
