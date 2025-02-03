using FluentValidation;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ItemProposalAPI.Services.Service
{
    public class PartyService : IPartyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreatePartyRequestDto> _addValidator;
        private readonly IValidator<UpdatePartyRequestDto> _updateValidator;

        public PartyService(IUnitOfWork unitOfWork, IValidator<CreatePartyRequestDto> addValidator, IValidator<UpdatePartyRequestDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<Result<IEnumerable<PartyWithoutUsersDto>>> GetAllAsync()
        {
            var parties = await _unitOfWork.PartyRepository.GetAllAsync();
            if (!parties.Any())
                return Result<IEnumerable<PartyWithoutUsersDto>>.Failure(ErrorType.NotFound, "No parties found");

            var partyDTOs = parties.Select(p => p.ToPartyWithoutUsersDto());
            return Result<IEnumerable<PartyWithoutUsersDto>>.Success(partyDTOs);
        }

        public async Task<Result<PartyDto>> GetByIdAsync(int partyId)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync(partyId, p => p.Users!);
            if (party == null)
                return Result<PartyDto>.Failure(ErrorType.NotFound, $"Party with ID:{partyId} does not exist.");

            return Result<PartyDto>.Success(party.ToPartyDto());
        }

        public async Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetPartyItemsAsync(int partyId)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync(partyId);
            if(party == null)
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"Party with ID:{partyId} does not exist");

            var partyItems = await _unitOfWork.ItemPartyRepository.GetPartyItemsAsync(partyId, null);
            if (!partyItems.Any())
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"Party with ID:{partyId} does now own shares of any item.");

            var partyItemDTOs = partyItems.Select(pi => pi.ToItemWithoutProposalsDto());

            return Result<IEnumerable<ItemWithoutProposalsDto>>.Success(partyItemDTOs);
        }

        public async Task<Result<Party>> AddAsync(CreatePartyRequestDto createPartyDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationResult = await _addValidator.ValidateAsync(createPartyDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Party>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var partyModel = createPartyDto.ToPartyFromCreateDto();

            await _unitOfWork.PartyRepository.AddAsync(partyModel);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Party>.Success(partyModel);
        }

        public async Task<Result<PartyWithoutUsersDto>> UpdateAsync(int partyId, UpdatePartyRequestDto updatePartyDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var existingParty = await _unitOfWork.PartyRepository.GetByIdAsync(partyId);
            if (existingParty == null)
                return Result<PartyWithoutUsersDto>.Failure(ErrorType.NotFound, $"Party with ID: {partyId} does not exist.");

            var validationResult = await _updateValidator.ValidateAsync(updatePartyDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<PartyWithoutUsersDto>.Failure(ErrorType.BadRequest, errorResponse);
            }

            _unitOfWork.PartyRepository.UpdateAsync(updatePartyDto.ToPartyFromUpdateDto(existingParty));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<PartyWithoutUsersDto>.Success(existingParty.ToPartyWithoutUsersDto());
        }

        public async Task<Result<Party>> DeleteAsync(int partyId)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedParty = await _unitOfWork.PartyRepository.DeleteAsync(partyId);
            if (deletedParty == null)
                return Result<Party>.Failure(ErrorType.NotFound, $"Party with ID: {partyId} does not exist.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Party>.Success(deletedParty);
        }
    }
}
