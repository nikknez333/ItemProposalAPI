using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;

namespace ItemProposalAPI.Mappers
{
    public static class AccountMapper
    {
        public static async Task<UserAccountDto> ToUserAccountDto(this User userModel, ITokenService tokenService)
        {
            return new UserAccountDto
            {
                Username = userModel.UserName,
                PartyId = userModel.PartyId,
                Token = await tokenService.CreateToken(userModel)
            };
        }

        public static User ToUserFromRegisterDto(this RegisterDto registerDto)
        {
            return new User
            {
                UserName = registerDto.Username,
                PartyId = registerDto.PartyId
            };
        }
    }
}
