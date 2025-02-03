using FluentValidation;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using System.Runtime.InteropServices;

namespace ItemProposalAPI.Services.Service
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateItemRequestDto> _addValidator;
        private readonly IValidator<UpdateItemRequestDto> _updateValidator;

        public ItemService(IUnitOfWork unitOfWork, IValidator<CreateItemRequestDto> addValidator, IValidator<UpdateItemRequestDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetAllAsync()
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsync();
            if (!items.Any())
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"No items found.");

            var itemDTOs = items.Select(i => i.ToItemWithoutProposalsDto());

            return Result<IEnumerable<ItemWithoutProposalsDto>>.Success(itemDTOs);
        }

        public async Task<Result<ItemWithoutProposalsDto>> GetByIdAsync(int itemId)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (item == null)
                return Result<ItemWithoutProposalsDto>.Failure(ErrorType.NotFound, $"Item with ID:{itemId} does not exist.");

            return Result<ItemWithoutProposalsDto>.Success(item.ToItemWithoutProposalsDto());
        }

        public async Task<Result<IEnumerable<PartyWithoutUsersDto>>> GetPartiesSharingItemAsync(int itemId)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (item == null)
                return Result<IEnumerable<PartyWithoutUsersDto>>.Failure(ErrorType.NotFound, $"Item with id:{itemId} does not exist.");

            var partiesSharingItem = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(itemId);
            if (partiesSharingItem == null)
                return Result<IEnumerable<PartyWithoutUsersDto>>.Failure(ErrorType.NotFound, $"Item with id:{itemId} is not shared with any party.");

            var partiesSharingItemDTOs = partiesSharingItem.Select(p => p.ToPartyWithoutUsersDto());

            return Result<IEnumerable<PartyWithoutUsersDto>>.Success(partiesSharingItemDTOs);
        }

        public async Task<Result<ItemNegotiationDto>> GetNegotiationDetails(int itemId)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if(item == null)
                return Result<ItemNegotiationDto>.Failure(ErrorType.NotFound, $"Item with ID:{itemId} does not exist");

            if (item.Share_Status != Status.Shared)
                return Result<ItemNegotiationDto>.Failure(ErrorType.BadRequest, $"Proposals can be retrieved only for shared items. Item ID:{itemId} is not shared.");

            var itemProposal = await _unitOfWork.ProposalRepository.GetNegotiationDetails(itemId, p => p.ProposalItemParties);
            if (itemProposal == null || !itemProposal.Any())
                return Result<ItemNegotiationDto>.Failure(ErrorType.NotFound, $"Item ID: {itemId} does not have any submitted proposal.");

            var itemNegotiationDTOs = item.ToItemNegotiationDto(itemProposal);

            return Result<ItemNegotiationDto>.Success(itemNegotiationDTOs);
        }

        public async Task<Result<Item>> AddAsync(CreateItemRequestDto createItemDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationResult = await _addValidator.ValidateAsync(createItemDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<Item>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var itemModel = createItemDto.ToItemFromCreateDto();

            await _unitOfWork.ItemRepository.AddAsync(itemModel);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Item>.Success(itemModel);
        }

        public async Task<Result<ItemWithoutProposalsDto>> UpdateAsync(int itemId, UpdateItemRequestDto updateItemDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
            if (existingItem == null)
                return Result<ItemWithoutProposalsDto>.Failure(ErrorType.NotFound, $"Item with ID:{itemId} does not exist");

            var validationResult = await _updateValidator.ValidateAsync(updateItemDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<ItemWithoutProposalsDto>.Failure(ErrorType.BadRequest, errorResponse);
            }

            _unitOfWork.ItemRepository.UpdateAsync(updateItemDto.ToItemFromUpdateDto(existingItem));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<ItemWithoutProposalsDto>.Success(existingItem.ToItemWithoutProposalsDto());
        }
        public async Task<Result<Item>> DeleteAsync(int itemId)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedItem = await _unitOfWork.ItemRepository.DeleteAsync(itemId);
            if (deletedItem == null)
                return Result<Item>.Failure(ErrorType.NotFound, $"Item with ID:{itemId} does not exist");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<Item>.Success(deletedItem);
        }

    }
}
