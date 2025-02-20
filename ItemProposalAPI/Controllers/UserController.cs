using ItemProposalAPI.DTOs.Item;
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


        /// <summary>
        /// Retrieves all items belonging to the currently authenticated user's party.
        /// </summary>
        /// <param name="query">Optional query parameters for filtering, sorting and pagination.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/user/party/items
        ///     
        /// </remarks>
        /// <returns>Returns a list of items associated with user's party.</returns>
        /// <response code="200">Successfully retrieved user's party items</response>
        /// <response code="404">User is not associated with any party or no matching items were found</response>
        [HttpGet("party/items")]
        [ProducesResponseType(typeof(IEnumerable<ItemWithoutProposalsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllMyPartyItems([FromQuery] QueryObject query)
        {
            var result = await _userService.GetMyPartyItemsAsync(User, query);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("proposals")]
        //TODO: FIX THIS
        public async Task<IActionResult> GetMyProposals([FromQuery] PaginationObject pagination)
        {
            var result = await _userService.GetMyProposals(User, pagination);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }
    }
}
