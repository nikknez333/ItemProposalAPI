using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;
using System.Security.Claims;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetAllAsync(PaginationObject pagination);
        Task<Result<ItemWithoutProposalsDto>> GetByIdAsync(int itemId);
        Task<Result<IEnumerable<PartyWithoutUsersDto>>> GetPartiesSharingItemAsync(int itemId, PaginationObject pagination);
        Task<Result<ItemNegotiationDto>> GetNegotiationDetails(int itemId, PaginationObject pagination, ClaimsPrincipal User);
        Task<Result<Item>> AddAsync(CreateItemRequestDto createItemDto);
        Task<Result<ItemWithoutProposalsDto>> UpdateAsync(int itemId, UpdateItemRequestDto updateItemDto);
        Task<Result<Item>> DeleteAsync(int itemId); 
    }
}
