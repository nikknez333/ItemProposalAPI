using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Retrieves a list of all items.
        /// </summary>
        /// <param name="pagination">Optional pagination object parameter (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/items
        /// 
        /// </remarks>
        /// <returns>Retuns a list of all items.</returns>
        /// <response code="200">Returns the list with details of all items.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403"></response>
        /// <response code="404">No items exist in system.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemWithoutProposalsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _itemService.GetAllAsync(pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves an item by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/items/1
        /// 
        /// </remarks>
        /// <returns>Returns retrieved item.</returns>
        /// <response code="200">Returns the item details.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403"></response>
        /// <response code="404">No item found with the specified ID.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ItemWithoutProposalsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _itemService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a list of parties that share the specified item.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item.</param>
        /// <param name="pagination">Optional pagination object parameter (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/items/1/parties
        /// 
        /// </remarks>
        /// <returns>Returns a list of parties that share the specified item.</returns>
        /// <response code="200">Returns the list of parties sharing the item.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403"></response>
        /// <response code="404">Item does not exist or is not shared between any parties.</response>
        [HttpGet("{itemId:int}/parties")]
        [ProducesResponseType(typeof(PartyWithoutUsersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPartiesSharingItem([FromRoute] int itemId, [FromQuery] PaginationObject pagination)
        {
            var result = await _itemService.GetPartiesSharingItemAsync(itemId, pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a list of negotiation details for the specified shared item.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="pagination">Optional pagination object parameter (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/items/1/proposals
        /// 
        /// </remarks>
        /// <returns>Returns a list of negotiation details for the specified shared item.</returns>
        /// <response code="200">Returns the negotiation details for the item.</response>
        /// <response code="400">Invalid request data (e.g., missing required fields, validation errors).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">The item does not exist or has no proposals.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpGet("{id:int}/proposals")]
        [Authorize(Roles = "UserPartyOwner,UserPartyEmployee")]
        [ProducesResponseType(typeof(ItemNegotiationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GetNegotiationDetails([FromRoute] int id, [FromQuery] PaginationObject pagination)
        {
            var result = await _itemService.GetNegotiationDetails(id, pagination, User);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new { Errors = result.Errors })
                };
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="createItemDto">The data transfer object containing the item details.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/items
        ///     {
        ///         "name": "Iphone 16"
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="201">The item was successfully created.</response>
        /// <response code="400">Invalid request data (e.g., missing required fields, validation errors).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to create resource.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ItemWithoutProposalsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Add([FromBody] CreateItemRequestDto createItemDto)
        {
            var result = await _itemService.AddAsync(createItemDto);
            if (!result.IsSuccess)
                return BadRequest(new
                {
                    Errors = result.Errors
                });

            return CreatedAtAction(nameof(GetById), new {id = result.Data.Id}, result.Data.ToItemWithoutProposalsDto());
        }

        /// <summary>
        /// Updates an existing item with new details.
        /// </summary>
        /// <param name="id">The unique identifier of the item to update.</param>
        /// <param name="updateItemDto">The data transfer object containing the updated item details.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/items/1
        ///     {
        ///         "name": "Iphone 16e"
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">The item was successfully updated.</response>
        /// <response code="400">Invalid request data (e.g., missing required fields, validation errors).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to update resource.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ItemWithoutProposalsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateItemRequestDto updateItemDto)
        {
            var result = await _itemService.UpdateAsync(id, updateItemDto);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                { 
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new {Errors = result.Errors})
                };
            }

            return Ok(result.Data);
        }
        /// <summary>
        /// Deletes an existing item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item to delete.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/items/1
        /// 
        /// </remarks>
        /// <returns>Returns no content.</returns>
        /// <response code="204">The item was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to delete the item.</response>
        /// <response code="404">The specified item does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _itemService.DeleteAsync(id);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
