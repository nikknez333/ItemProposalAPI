using FluentValidation;
using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Service
{
    public class ItemPartyService : IItemPartyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateItemPartyRequestDto> _addValidator;

        public ItemPartyService(IUnitOfWork unitOfWork, IValidator<CreateItemPartyRequestDto> addValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
        }
        public async Task<Result<IEnumerable<ItemPartyDto>>> GetAllAsync(PaginationObject pagination)
        {
            var itemParties = await _unitOfWork.ItemPartyRepository.GetAllAsync(pagination.PageNumber, pagination.PageSize, ip => ip.Party, ip => ip.Item);
            if (itemParties == null || !itemParties.Any())
                return Result<IEnumerable<ItemPartyDto>>.Failure(ErrorType.NotFound, $"No Items is being owned by any Party.");

            var ItemPartyDTOs = itemParties.Select(ip => ip.ToItemPartyDto());

            return Result<IEnumerable<ItemPartyDto>>.Success(ItemPartyDTOs);
        }

        public async Task<Result<ItemPartyDto>> GetByIdAsync(int partyId, int itemId)
        {
            var itemParty = await _unitOfWork.ItemPartyRepository.GetByIdAsync(partyId, itemId, ip => ip.Party, ip => ip.Item);
            if (itemParty == null)
                return Result<ItemPartyDto>.Failure(ErrorType.NotFound, $"ItemParty with party ID:{partyId} / item ID: {itemId} does not exist.");

            return Result<ItemPartyDto>.Success(itemParty.ToItemPartyDto());
        }

        public async Task<Result<ItemParty>> AddAsync(CreateItemPartyRequestDto createItemPartyDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationResult = await _addValidator.ValidateAsync(createItemPartyDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<ItemParty>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var itemPartyModel = createItemPartyDto.ToItemPartyFromCreateDto();

            await _unitOfWork.ItemPartyRepository.AddItemPartyAsync(itemPartyModel);

            await _unitOfWork.SaveChangesAsync();

            var partiesSharingItem = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(createItemPartyDto.ItemId);
            if (partiesSharingItem.Count() > 1)
            {
                var itemModel = await _unitOfWork.ItemRepository.GetByIdAsync(createItemPartyDto.ItemId);
                itemModel.Share_Status = Status.Shared;
                _unitOfWork.ItemRepository.UpdateAsync(itemModel);
                await _unitOfWork.SaveChangesAsync();
            }
 
            await _unitOfWork.CommitAsync();

            return Result<ItemParty>.Success(itemPartyModel);
        }

        public async Task<Result<ItemParty>> DeleteAsync(int partyId, int itemId)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var loadedItemParty = await _unitOfWork.ItemPartyRepository.GetByIdAsync(partyId, itemId);
            if (loadedItemParty == null)
                return Result<ItemParty>.Failure(ErrorType.NotFound, $"ItemParty with party ID:{partyId} / item ID: {itemId} does not exist.");

            var deletedItemParty = await _unitOfWork.ItemPartyRepository.RemoveItemPartyAsync(loadedItemParty.PartyId, loadedItemParty.ItemId);
            
            await _unitOfWork.SaveChangesAsync();

            var partiesSharingItem = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(itemId);
            if(partiesSharingItem.Count() <= 1)
            {
                var itemModel = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
                itemModel.Share_Status = Status.Not_Shared;
                _unitOfWork.ItemRepository.UpdateAsync(itemModel);

                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitAsync();

            return Result<ItemParty>.Success(deletedItemParty);
        }
    }
}
