using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserWithoutProposalsDto>>> GetAllAsync(/*QueryObject queryObject*/);
        Task<Result<UserDto>> GetByIdAsync(int userId);
        Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetMyPartyItemsAsync(int userId, QueryObject query);
        Task<Result<User>> AddAsync(CreateUserRequestDto createUserDto);
        Task<Result<UserWithoutProposalsDto>> UpdateAsync(int id, UpdateUserRequestDto updateUserDto);
        Task<Result<User>> DeleteAsync(int id);
    }
}
