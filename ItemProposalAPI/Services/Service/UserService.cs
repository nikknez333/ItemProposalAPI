using FluentValidation;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;

namespace ItemProposalAPI.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateUserRequestDto> _validator;
        private readonly IValidator<UpdateUserRequestDto> _updateValidator;

        public UserService(IUnitOfWork unitOfWork, IValidator<CreateUserRequestDto> validator, IValidator<UpdateUserRequestDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<IEnumerable<UserWithoutProposalsDto>>> GetAllAsync(/*QueryObject queryObject*/)
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(/*queryObject,*/);
            if (!users.Any())
                return Result<IEnumerable<UserWithoutProposalsDto>>.Failure(ErrorType.NotFound, "No users found");

            var userDTOs = users.Select(u => u.ToUserWithoutProposalsDto());
            return Result<IEnumerable<UserWithoutProposalsDto>>.Success(userDTOs);
        }

        public async Task<Result<UserDto>> GetByIdAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, u => u.Proposals!);
            if (user == null)
                return Result<UserDto>.Failure(ErrorType.NotFound, $"User with ID:{userId} does not exist.");

            return Result<UserDto>.Success(user.ToUserDto());
        }

        public async Task<Result<IEnumerable<ItemWithoutProposalsDto>>> GetMyPartyItemsAsync(int userId, QueryObject query)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"User with id:{userId} does not exist.");

            if (user.PartyId == null)
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"User with id:{userId} is not associated with any party.");

            var partyItems = await _unitOfWork.ItemPartyRepository.GetPartyItemsAsync(user.PartyId, query);
            if (partyItems == null || !partyItems.Any())
                return Result<IEnumerable<ItemWithoutProposalsDto>>.Failure(ErrorType.NotFound, $"No items owned by Party with ID:{user.PartyId} match provided filter conditions.");

            var partyItemDTOs = partyItems.Select(p => p.ToItemWithoutProposalsDto());
            return Result<IEnumerable<ItemWithoutProposalsDto>>.Success(partyItemDTOs);
        }

        public async Task<Result<User>> AddAsync(CreateUserRequestDto createUserDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var validationResult = await _validator.ValidateAsync(createUserDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<User>.Failure(ErrorType.BadRequest, errorResponse);
            }

            var userModel = createUserDto.ToUserFromCreateDto();

            await _unitOfWork.UserRepository.AddAsync(userModel);
            
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return Result<User>.Success(userModel);
        }

        public async Task<Result<UserWithoutProposalsDto>> UpdateAsync(int id, UpdateUserRequestDto updateUserDto)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (existingUser == null)
                return Result<UserWithoutProposalsDto>.Failure(ErrorType.NotFound, $"User with id:{id} does not exist.");

            var validationResult = await _updateValidator.ValidateAsync(updateUserDto);
            if(!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return Result<UserWithoutProposalsDto>.Failure(ErrorType.BadRequest, errorResponse);
            }

            _unitOfWork.UserRepository.UpdateAsync(updateUserDto.ToUserFromUpdateDto(existingUser));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<UserWithoutProposalsDto>.Success(existingUser.ToUserWithoutProposalsDto());
        }

        public async Task<Result<User>> DeleteAsync(int id)
        {
            var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedUser = await _unitOfWork.UserRepository.DeleteAsync(id);
            if (deletedUser == null)
                return Result<User>.Failure(ErrorType.NotFound, $"User with id:{id} does not exist.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<User>.Success(deletedUser);
        }
    }
}
