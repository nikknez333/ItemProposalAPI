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
                Username = userModel.UserName,
                Proposals = userModel.Proposals!.Select(p => p.ToProposalDto()).ToList()
            };
        }

        public static UserWithoutProposalsDto ToUserWithoutProposalsDto(this User userModel)
        {
            return new UserWithoutProposalsDto
            {
                Id = userModel.Id,
                Username = userModel.UserName,
            };
        }
    }
}
