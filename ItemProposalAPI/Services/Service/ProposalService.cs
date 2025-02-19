using FluentValidation;
using ItemProposalAPI.ClaimsExtension;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ItemProposalAPI.Services.Service
{
    public class ProposalService : IProposalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProposalRequestDto> _addValidator;
        private readonly IValidator<CreateCounterProposalRequestDto> _counterAddValidator;
        private readonly IValidator<UpdateProposalRequestDto> _updateValidator;
        private readonly IValidator<ReviewProposalDto> _reviewProposalValidator;
        private readonly UserManager<User> _userManager;    

        public ProposalService(IUnitOfWork unitOfWork, IValidator<CreateProposalRequestDto> addValidator, IValidator<CreateCounterProposalRequestDto> counterAddValidator,
            IValidator<UpdateProposalRequestDto> updateValidator, IValidator<ReviewProposalDto> reviewProposalValidator, UserManager<User> userManager)
            
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _counterAddValidator = counterAddValidator;
            _updateValidator = updateValidator;
            _reviewProposalValidator = reviewProposalValidator;
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<ProposalDto>>> GetAllAsync(PaginationObject pagination)
        {
            var proposals = await _unitOfWork.ProposalRepository.GetAllAsync(pagination.PageNumber, pagination.PageSize, p => p.ProposalItemParties);
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

        public async Task<Result<Proposal>> AddAsync(CreateProposalRequestDto createProposalDto, ClaimsPrincipal User)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationContext = new ValidationContext<CreateProposalRequestDto>(createProposalDto);
            validationContext.RootContextData["Username"] = User.GetUsername();

            var validationResult = await _addValidator.ValidateAsync(validationContext);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Proposal>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var proposalModel = createProposalDto.ToProposalFromCreateDto(userId);

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

            var myPartyId = validationContext.RootContextData["UserPartyId"] as int?;

            var myPaymentRatio = addedProposal.ProposalItemParties.FirstOrDefault(x => x.ProposalId == addedProposal.Id && x.ItemId == addedProposal.ItemId && x.PartyId == myPartyId);
            myPaymentRatio.Response = Proposal_Status.Accepted;
            myPaymentRatio.UserId = userId;

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return Result<Proposal>.Success(proposalModel);
        }

        public async Task<Result<Proposal>> AddCounterProposalAsync(int proposalId, CreateCounterProposalRequestDto createCounterProposalDto, ClaimsPrincipal User)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationContext = new ValidationContext<CreateCounterProposalRequestDto>(createCounterProposalDto);
            validationContext.RootContextData["ProposalId"] = proposalId;
            validationContext.RootContextData["Username"] = User.GetUsername();

            var validationResult = await _counterAddValidator.ValidateAsync(validationContext);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Proposal>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var ProposalToCounter = validationContext.RootContextData["ProposalToCounter"] as Proposal;
            if (ProposalToCounter == null)
            {
                return Result<Proposal>.Failure(ErrorType.BadRequest, $"Cannot counter proposal with ID:{proposalId}, because it does not exist.");
            }

            string? userId = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var counterProposalModel = createCounterProposalDto.ToCounterProposalFromCreateDto(ProposalToCounter, userId);
            var addedCounterProposal = await _unitOfWork.ProposalRepository.AddAsync(counterProposalModel);
            await _unitOfWork.SaveChangesAsync();

            foreach (var ratio in createCounterProposalDto.PaymentRatios)
            {
                var proposalItemParty = new ProposalItemParty
                {
                    ProposalId = addedCounterProposal.Id,
                    ItemId = ProposalToCounter.ItemId,
                    PartyId = ratio.PartyId,
                    PaymentType = ratio.PaymentType,
                    PaymentAmount = ratio.PaymentAmount,
                };

                await _unitOfWork.ProposalItemPartyRepository.AddAsync(proposalItemParty);
            }
            await _unitOfWork.SaveChangesAsync();

            int? myPartyId = validationContext.RootContextData["UserPartyId"] as int?;

            var myPaymentRatio = addedCounterProposal.ProposalItemParties.FirstOrDefault(x => x.ProposalId == addedCounterProposal.Id && x.ItemId == addedCounterProposal.ItemId && x.PartyId == myPartyId);
            myPaymentRatio.Response = Proposal_Status.Accepted;
            myPaymentRatio.UserId = userId;

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Proposal>.Success(counterProposalModel);
        }

        public async Task<Result<ProposalDto>> ReviewProposalAsync(int proposalId, ReviewProposalDto reviewProposalDto, ClaimsPrincipal User)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();
            //deterimine does proposal for which you want to review proposal exists
            var proposalToEvaluate = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId, p => p.ProposalItemParties);
            if (proposalToEvaluate == null)
                return Result<ProposalDto>.Failure(ErrorType.NotFound, $"Proposal with ID: {proposalId} does not exist.");

            //validation checks
            var validationContext = new ValidationContext<ReviewProposalDto>(reviewProposalDto);
            validationContext.RootContextData["ProposalId"] = proposalId;
            validationContext.RootContextData["ProposalToEvaluate"] = proposalToEvaluate;
            validationContext.RootContextData["Username"] = User.GetUsername();

            var validationResult = await _reviewProposalValidator.ValidateAsync(validationContext);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<ProposalDto>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var user = validationContext.RootContextData["User"] as User;
            var userPartyId = user.PartyId;

            //get payment ratio for 
            var ratio = proposalToEvaluate.ProposalItemParties.FirstOrDefault(x => x.ProposalId == proposalId && x.ItemId == proposalToEvaluate.ItemId && x.PartyId == userPartyId);

            //if you want to accept proposal, set response status of your party mentioned in payment ratios to accept
            if (reviewProposalDto.Response == Proposal_Status.Accepted)
            {
                ratio.Response = Proposal_Status.Accepted;
                ratio.UserId = user.Id;

                var allAccepted = proposalToEvaluate.ProposalItemParties.All(pip => pip.Response == Proposal_Status.Accepted);
                if (allAccepted)
                    proposalToEvaluate.Proposal_Status = Proposal_Status.Accepted;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result<ProposalDto>.Success(proposalToEvaluate.ToProposalDto());
            }
            else if(reviewProposalDto.Response == Proposal_Status.Rejected)
            {
                ratio.Response = Proposal_Status.Rejected;
                ratio.UserId = user.Id;

                proposalToEvaluate.Proposal_Status = Proposal_Status.Rejected;

                await _unitOfWork.SaveChangesAsync();

                var counterProposalDto = reviewProposalDto.ToCounterProposalFromReviewDto();

                var result = await AddCounterProposalAsync(proposalId, counterProposalDto, User);
                if (!result.IsSuccess)
                    return Result<ProposalDto>.Failure(result.ErrorType, result.Errors);

                return Result<ProposalDto>.Success(proposalToEvaluate.ToProposalDto());
            }

            return Result<ProposalDto>.Failure(ErrorType.BadRequest, $"Invalid response for Proposal with ID:{proposalId}. Allowed Responses: Accepted, Rejected");
        }

        public async Task<Result<ProposalDto>> UpdateAsync(int proposalId, UpdateProposalRequestDto updateProposalDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var existingProposal = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalId, p => p.ProposalItemParties);
            if (existingProposal == null)
                return Result<ProposalDto>.Failure(ErrorType.NotFound, $"Proposal with ID: {proposalId} does not exist.");

            var validationResult = await _updateValidator.ValidateAsync(updateProposalDto);
            if (!validationResult.IsValid)
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
