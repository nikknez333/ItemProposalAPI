using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/item-parties")]
    [ApiController]
    [Authorize(Roles = "UserPartyOwner")]
    public class ItemPartyController : ControllerBase
    {
        private readonly IItemPartyService _itemPartyService;

        public ItemPartyController(IItemPartyService itemPartyService)
        {
            _itemPartyService = itemPartyService;
        }

        /// <summary>
        /// Retrieves a list of all item-party relationships.
        /// </summary>
        /// <param name="pagination">Optional pagination object parameter (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/item-parties
        ///     
        /// </remarks> 
        /// <returns>Returns a list of all item-party relationships.</returns>
        /// <response code="200">Successfully retrieved the list of item-parties</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">No item-party exists in system.</response>
        /// <response code="500">Unexpected error while processing request.</response>
        [HttpGet]
        [Authorize(Roles = "UserPartyOwner,UserPartyEmployee")]
        [ProducesResponseType(typeof(IEnumerable<ItemPartyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _itemPartyService.GetAllAsync(pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves an item-party relationship by its party ID and item ID.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="itemId">The unique identifier of the item.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/item-parties/1/2
        /// 
        /// </remarks>
        /// <returns>Returns the requested item-party relationship.</returns>
        /// <response code="200">Successfully retrieved the item-party relationship.</response>
        /// <response code="404">Item-party relationship not found.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpGet("{partyId:int}/{itemId:int}")]
        [ProducesResponseType(typeof(ItemPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GetById([FromRoute] int partyId, [FromRoute] int itemId)
        {
            var result = await _itemPartyService.GetByIdAsync(partyId, itemId);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new item-party relationship.
        /// </summary>
        /// <param name="itemPartyDto">The data transfer object containing the item-party details.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/item-parties
        ///     {
        ///         "itemId":"2",
        ///         "partyId: "3"
        ///     }
        /// 
        /// </remarks>
        /// <returns>Returns successfully created the item-party relationship</returns>
        /// <response code="201">Successfully created the item-party relationship.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks the necessary permissions.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ItemPartyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Add([FromBody] CreateItemPartyRequestDto itemPartyDto)
        {
            var result = await _itemPartyService.AddAsync(itemPartyDto);
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            return CreatedAtAction(nameof(GetById), new {result.Data.PartyId,  result.Data.ItemId}, result.Data.ToItemPartyAddResultDto());
        }

        /// <summary>
        /// Deletes an item-party relationship.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="itemId">The unique identifier of the item.</param>
        /// <remarks>
        /// 
        ///     DELETE /api/item-parties/1/1
        /// 
        /// </remarks>
        /// <returns>Returns no content.</returns>
        /// <response code="204">Successfully deleted the item-party relationship.</response>
        /// <response code="404">The specified item-party relationship does not exist.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpDelete("{partyId:int}/{itemId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int partyId, [FromRoute] int itemId)
        {
            var result = await _itemPartyService.DeleteAsync(partyId, itemId);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
