using FluentValidation;
using ItemProposalAPI.ClaimsExtension;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItemProposalAPI.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetMyPartyItemsAsync(ClaimsPrincipal User, QueryObject query)
        {      
            var username = User.GetUsername();
            if (username.Equals("Unknown Username"))
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"Can't retrieve logged in user!");

            var user = await _userManager.FindByNameAsync(username);
            if (user.PartyId == null)
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"User:{username} is not associated with any party.");

            var partyItems = await _unitOfWork.ItemPartyRepository.GetPartyItemsAsync(user.PartyId, query);
            if (partyItems == null || !partyItems.Any())
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"No items owned by Party with ID:{user.PartyId} match provided filter conditions.");

            var partyItemDTOs = partyItems.Select(p => p.ToItemWithoutProposalsDto());
            return Result<IEnumerable<ItemWithoutProposalsDto>>.Success(partyItemDTOs);
        }

        public async Task<Result<UserDto>> GetMyProposals(ClaimsPrincipal User, PaginationObject pagination)
        {
            var username = User.GetUsername();
            if (username.Equals("Unknown Username"))
                return Result<UserDto>.Failure(ErrorType.NotFound, $"Can't retireve logged in user");

            var user = await _userManager.FindByNameAsync(username);
           
            var userDto = user.ToUserDto();

            return Result<UserDto>.Success(userDto);
        }
    }
}
