using FluentValidation;
using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.Validation;
using ItemProposalAPI.Validation.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItemProposalAPI.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<User> userManager, IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator, SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _tokenService = tokenService;
        }

        public async Task<Result<UserAccountDto>> Register(RegisterDto registerDto)
        {
            try
            {
                //validate register POST payload data
                var validationResult = await _registerValidator.ValidateAsync(registerDto);
                if (!validationResult.IsValid)
                {
                    var errorResponse = validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    });

                    return Result<UserAccountDto>.Failure(ErrorType.BadRequest, errorResponse);
                }

                var user = registerDto.ToUserFromRegisterDto();
                //Identity register user and hash password
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                //check register user result 
                if (createdUser.Succeeded)
                {
                    //Check did user registered with party who employs him or not and give him role based on that, check result 
                    if (user.PartyId.HasValue)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, "UserPartyEmployee");

                        if (roleResult.Succeeded)
                            return await Result<UserAccountDto>.Success(user.ToUserAccountDto(_tokenService));
                        
                        return Result<UserAccountDto>.Failure(ErrorType.GeneralError, roleResult.Errors);
                    }
                    else
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, "UserUnemployed");

                        if (roleResult.Succeeded)
                            return await Result<UserAccountDto>.Success(user.ToUserAccountDto(_tokenService));

                        return Result<UserAccountDto>.Failure(ErrorType.GeneralError, roleResult.Errors);
                    }
                }
                else
                {
                    return Result<UserAccountDto>.Failure(ErrorType.GeneralError, createdUser.Errors);
                }
            }
            //cover some other unexpected server error
            catch (Exception ex)
            {
                return Result<UserAccountDto>.Failure(ErrorType.GeneralError, ex);
            }
         }

        public async Task<Result<UserAccountDto>> Login(LoginDto loginDto)
        {
            try
            {
                var validationResult = await _loginValidator.ValidateAsync(loginDto);
                if(!validationResult.IsValid)
                {
                    var errorResponse = validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    });

                    return Result<UserAccountDto>.Failure(ErrorType.BadRequest, errorResponse);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

                if (user == null)
                    return Result<UserAccountDto>.Failure(ErrorType.Unauthorized, $"Invalid credentials!");

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!signInResult.Succeeded)
                    return Result<UserAccountDto>.Failure(ErrorType.Unauthorized, $"Username not found and/or password is incorrect!");

                return await Result<UserAccountDto>.Success(user.ToUserAccountDto(_tokenService));
            }
            catch(Exception ex)
            {
                return Result<UserAccountDto>.Failure(ErrorType.GeneralError, ex);
            }
        }
    }
}
