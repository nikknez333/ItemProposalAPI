using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.DTOs.User;
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
    [Route("api/parties")]
    [ApiController]
    [Authorize(Roles = "UserPartyOwner")]
    public class PartyController : ControllerBase
    {
        private readonly IPartyService _partyService;

        public PartyController(IPartyService partyService)
        {
            _partyService = partyService;
        }

        /// <summary>
        /// Retrieves a list of all parties.
        /// </summary>
        /// <param name="pagination">Pagination object parameter (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/parties
        ///     
        /// </remarks> 
        /// <returns>Returns a list of all parties.</returns>
        /// <response code="200">Successfully retrieved the list of parties</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">No parties exists in system.</response>
        [HttpGet]
        [Authorize(Roles = "UserPartyEmployee, UserPartyOwner")]
        [ProducesResponseType(typeof(IEnumerable<PartyWithoutUsersDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _partyService.GetAllAsync(pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific party by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the party.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/parties/1
        ///     
        /// </remarks> 
        /// <returns>Returns retireved party.</returns>
        /// <response code="200">Successfully retrieved the party.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">Party with the given ID does not exist.</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "UserPartyEmployee, UserPartyOwner")]
        [ProducesResponseType(typeof(PartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _partyService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all items owned or shared by a specific party.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/parties/1/items
        ///     
        /// </remarks> 
        /// <returns>Returns retrieved party's items</returns>
        /// <response code="200">Successfully retrieved the party's items.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">Party with the given ID does not exist or has no items.</response>
        [HttpGet("{partyId:int}/items")]
        [Authorize(Roles = "UserPartyEmployee, UserPartyOwner")]
        [ProducesResponseType(typeof(IEnumerable<ItemWithoutProposalsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPartyItems([FromRoute] int partyId)
        {
            var result = await _partyService.GetPartyItemsAsync(partyId);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }


        /// <summary>
        /// Creates a new party.
        /// </summary>
        /// <param name="createPartyDto">The party create request data transfer object containing the party details.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /api/parties
        ///     {
        ///         "name": "Tesla inc."
        ///     }
        ///     
        /// </remarks> 
        /// <returns>Returns created party object.</returns>
        /// <response code="201">The party was successfully created.</response>
        /// <response code="400">Invalid request data (e.g., missing required fields, validation errors).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to create resource.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PartyWithoutUsersDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Produces("application/json")]
        public async Task<IActionResult> Add([FromBody] CreatePartyRequestDto createPartyDto)
        {
            var result = await _partyService.AddAsync(createPartyDto);
            if (!result.IsSuccess)
                return BadRequest(new
                {
                    Errors = result.Errors
                });

            return CreatedAtAction(nameof(GetById), new {id = result.Data.Id}, result.Data.ToPartyWithoutUsersDto());
        }

        /// <summary>
        /// Updates an existing party.
        /// </summary>
        /// <param name="id">The unique identifier of the party to update.</param>
        /// <param name="updatePartyDto">The party update request data transfer object containing the new values</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     PUT /api/parties/1
        ///     {
        ///         "name": "SpaceX inc."
        ///     }
        ///     
        /// </remarks> 
        /// <returns>Returns updated party object.</returns>
        /// <response code="200">Successfully updated the party.</response>
        /// <response code="400">Invalid request data (e.g., missing required fields, validation errors).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to update resource.</response>
        /// <response code="404">The party with the given ID does not exist.</response>
        /// <response code="500">Unexpected error while processing the request</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PartyWithoutUsersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePartyRequestDto updatePartyDto)
        {
            var result = await _partyService.UpdateAsync(id, updatePartyDto);
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
        /// Deletes a party by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the party to delete.</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     DELETE /api/parties/1
        ///
        /// </remarks> 
        /// <returns>Returns no content.</returns>
        /// <response code="204">Successfully deleted the party</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to delete resource.</response>
        /// <response code="404">The party with the given ID does not exist.</response>
        /// <response code="500">Unexpected error while processing the request</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _partyService.DeleteAsync(id);
            if(!result.IsSuccess)
                return NotFound(result.Errors); 

            return NoContent();
        }
    }
}
