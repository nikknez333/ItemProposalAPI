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


        /// <summary>
        /// Authenticates a user by validating the login credentials (username and password).
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing the required username and password for authentication.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /api/account/login
        ///     {
        ///         "username": "nikKnez337"
        ///         "password": "kneZnik733?"
        ///     }
        ///     
        /// </remarks>
        /// <returns>Returns the authenticated user.</returns>
        /// <response code="200">Returns publicly available user account data and an authentication token if login is successfull.</response>
        /// <response code="400">Returns validation errors (e.g missing username or password).</response>
        /// <response code="401">Invalid login credentials (incorrect login or username).</response>
        /// <response code="500">An unexpected error that occured while processing requst.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
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

        /// <summary>
        /// Registers a new user in the system with the provided account details.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object containing the required username, password, and optional party ID for the new registration.</param>
        /// <remarks>
        /// Sample request:
        ///    
        ///     POST /api/account/register
        ///     {
        ///         "username": "nikKnez337",
        ///         "password": "kneZnik733?",
        ///         "partyId": "1"
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="201">Returns successfully created the user account with appropriate assigned role.</response>
        /// <response code="400">Returns validation errors (e.g. invalid input, such as missing username/password or username already taken).</response>
        /// <response code="500">An unexpected error that occured while processing requst, such as role assignment failure or unexpected database error.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserAccountDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        //TODO: FIX STATUS CODES IF NEEDED, FOR EXAMPLE USERNAME IS TAKEN RETURN 409 INSTEAD OF 400
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

            return CreatedAtAction(nameof(Register), new { id = result.Data.Username}, result.Data);
        }
    }
}
