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
                Username = userModel.Username
            };
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto createUserDto)
        {
            return new User
            {
                Username = createUserDto.Username,
            };
        }

        public static User ToUserFromUpdateDto(this User user, UpdateUserRequestDto updateUserDto)
        {
           user.Username = updateUserDto.Username;

           return user;
        }
    }
}
