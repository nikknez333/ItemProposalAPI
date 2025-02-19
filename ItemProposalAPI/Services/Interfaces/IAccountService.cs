using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.Models;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Result<UserAccountDto>> Register(RegisterDto registerDto);
        //Task<Result<User>> EditProfile()
        Task<Result<UserAccountDto>> Login(LoginDto loginDto);
    }
}
