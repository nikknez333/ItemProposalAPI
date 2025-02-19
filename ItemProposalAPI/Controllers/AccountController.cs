using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _accountService.Login(loginDto);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    ErrorType.Unauthorized => Unauthorized(result.Errors),
                    ErrorType.GeneralError => StatusCode(500, new { Errors = result.Errors }),
                };
            }

            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _accountService.Register(registerDto);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    ErrorType.GeneralError => StatusCode(500, new {Errors = result.Errors})
                };
            }

            return Ok(result.Data);
        }
    }
}
