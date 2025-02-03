using FluentValidation;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Service
{
    public class ProposalService : IProposalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProposalRequestDto> _addValidator;
        private readonly IValidator<CreateCounterProposalRequestDto> _counterAddValidator;
        private readonly IValidator<UpdateProposalRequestDto> _updateValidator;

        public ProposalService(IUnitOfWork unitOfWork, IValidator<CreateProposalRequestDto> addValidator, IValidator<CreateCounterProposalRequestDto> counterAddValidator, IValidator<UpdateProposalRequestDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _counterAddValidator = counterAddValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<IEnumerable<ProposalDto>>> GetAllAsync()
        {
            var proposals = await _unitOfWork.ProposalRepository.GetAllAsync(p => p.ProposalItemParties);
            if (!proposals.Any())
                return Result<IEnumerable<ProposalDto>>.Failure(ErrorType.NotFound, "No proposals found.");

            var proposalDTOs = proposals.Select(p => p.ToProposalDto());

            return Result<IEnumerable<ProposalDto>>.Success(proposalDTOs);
        }

        public async Task<Result<ProposalDto>> GetByIdAsync(int proposalId)
        {
            var proposal = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId, p => p.ProposalItemParties);
            if (proposal == null)
                return Result<ProposalDto>.Failure(ErrorType.NotFound, $"Proposal with ID:{proposalId} does not exist.");

            return Result<ProposalDto>.Success(proposal.ToProposalDto());
        }

        public async Task<Result<Proposal>> AddAsync(CreateProposalRequestDto createProposalDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationResult = await _addValidator.ValidateAsync(createProposalDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Proposal>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var proposalModel = createProposalDto.ToProposalFromCreateDto();

            var addedProposal = await _unitOfWork.ProposalRepository.AddAsync(proposalModel);

            await _unitOfWork.SaveChangesAsync();

            foreach(var ratio in createProposalDto.PaymentRatios)
            {
                var proposalItemParty = new ProposalItemParty
                {
                    ProposalId = addedProposal.Id,
                    ItemId = createProposalDto.ItemId,
                    PartyId = ratio.PartyId,
                    PaymentType = ratio.PaymentType,
                    PaymentAmount = ratio.PaymentAmount
                };

                await _unitOfWork.ProposalItemPartyRepository.AddAsync(proposalItemParty);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Proposal>.Success(proposalModel);
        }

        public async Task<Result<Proposal>> AddCounterProposalAsync(int proposalId, CreateCounterProposalRequestDto createCounterProposalDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationContext = new ValidationContext<CreateCounterProposalRequestDto>(createCounterProposalDto);
            validationContext.RootContextData["ProposalId"] = proposalId;

            var validationResult = await _counterAddValidator.ValidateAsync(createCounterProposalDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Proposal>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var originalProposal = validationContext.RootContextData["OriginalProposal"] as Proposal;
            if (originalProposal == null)
            {
                return Result<Proposal>.Failure(ErrorType.BadRequest, "The original proposal does not exist.");
            }

            var counterProposalModel = createCounterProposalDto.ToCounterProposalFromCreateDto(originalProposal);
            var addedCounterProposal = await _unitOfWork.ProposalRepository.AddAsync(counterProposalModel);
            await _unitOfWork.SaveChangesAsync();

            foreach (var ratio in createCounterProposalDto.PaymentRatios)
            {
                var proposalItemParty = new ProposalItemParty
                {
                    ProposalId = addedCounterProposal.Id,
                    ItemId = originalProposal.ItemId,
                    PartyId = ratio.PartyId,
                    PaymentType = ratio.PaymentType,
                    PaymentAmount = ratio.PaymentAmount,
                };

                await _unitOfWork.ProposalItemPartyRepository.AddAsync(proposalItemParty);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Proposal>.Success(counterProposalModel);
        }

        public async Task<Result<ProposalDto>> UpdateAsync(int proposalId, UpdateProposalRequestDto updateProposalDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var existingProposal = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId, p => p.ProposalItemParties);
            if (existingProposal == null)
                return Result<ProposalDto>.Failure(ErrorType.NotFound, $"Proposal with ID: {proposalId} does not exist.");

            var validationResult = await _updateValidator.ValidateAsync(updateProposalDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    FieldName = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<ProposalDto>.Failure(ErrorType.BadRequest, errorResponse);
            }

            _unitOfWork.ProposalRepository.UpdateAsync(updateProposalDto.ToProposalFromUpdateDto(existingProposal));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<ProposalDto>.Success(existingProposal.ToProposalDto());
        }

        public async Task<Result<Proposal>> DeleteAsync(int proposalId)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();
            
            var deletedProposal = await _unitOfWork.ProposalRepository.DeleteAsync(proposalId);
            if (deletedProposal == null)
                return Result<Proposal>.Failure(ErrorType.NotFound, $"Proposal with ID: {proposalId} does not exist.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Proposal>.Success(deletedProposal);
        }
    }
}
