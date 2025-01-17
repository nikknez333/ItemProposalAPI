using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Models;
using System.Runtime.CompilerServices;

namespace ItemProposalAPI.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToUserDto(this User userModel)
        {
            return new UserDto
            {
                Id = userModel.Id,
                PartyId = userModel.PartyId,
                Username = userModel.Username,
                Proposals = userModel.Proposals!.Select(p => p.ToProposalDto()).ToList()
            };
        }

        public static UserWithoutProposalsDto ToUserWithoutProposalsDto(this User userModel)
        {
            return new UserWithoutProposalsDto
            {
                Id = userModel.Id,
                PartyId = userModel.PartyId,
                Username = userModel.Username,
            };
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto createUserDto)
        {
            return new User
            {
                PartyId = createUserDto.PartyId,
                Username = createUserDto.Username,
            };
        }

        public static User ToUserFromUpdateDto(this UpdateUserRequestDto updateUserDto, User user)
        {
           user.PartyId = updateUserDto.PartyId;
           user.Username = updateUserDto.Username;

           return user;
        }
    }
}
