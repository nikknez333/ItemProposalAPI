using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;
using System.Security.Claims;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetMyPartyItemsAsync(ClaimsPrincipal User, QueryObject query);
        Task<Result<UserDto>> GetMyProposals(ClaimsPrincipal User, PaginationObject pagination);
    }
}
