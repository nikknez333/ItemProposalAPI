using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ItemProposalAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("party/items")]
        public async Task<IActionResult> GetAllMyPartyItems([FromQuery] QueryObject query)
        {
            var result = await _userService.GetMyPartyItemsAsync(User, query);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("proposals")]
        public async Task<IActionResult> GetMyProposals([FromQuery] PaginationObject pagination)
        {
            var result = await _userService.GetMyProposals(User, pagination);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }
    }
}
