using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace ItemProposalAPI.Controllers
{
    [Route("api/proposals")]
    [ApiController]
    [Authorize(Roles = "UserPartyOwner,UserPartyEmployee")]
    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;

        public ProposalController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }


        /// <summary>
        /// Retrieves a list of all proposals.
        /// </summary>
        /// <param name="pagination">Optional pagination parameters (page number and page size).</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /api/proposals?pageNumber=1&pageSize=10
        /// 
        /// </remarks>
        /// <returns>Returns a list of all proposals.</returns>
        /// <response code="200">Successfully retrieved the list of proposals.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions to access resource.</response>
        /// <response code="404">No proposals found.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProposalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _proposalService.GetAllAsync(pagination);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a proposal by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the proposal to retrieve.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/proposals/1
        /// 
        /// </remarks>
        /// <returns>Returns retrieved proposal.</returns>
        /// <response code="200">Successfully retrieved the proposal details.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions.</response>
        /// <response code="404">Proposal with the given ID does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProposalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _proposalService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new proposal for an item.
        /// </summary>
        /// <param name="proposalDto">The data transfer object containing the proposal details.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/proposals
        ///     {
        ///         "itemId": "1",
        ///         "comment": "Proposal comment here",
        ///         "paymentRatios": [
        ///             {
        ///                 "partyId": 1,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 40
        ///             },
        ///             {
        ///                 "partyId": 2,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 60
        ///             }
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="201">Successfully created a new proposal.</response>
        /// <response code="400">Invalid request data or validation errors.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProposalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromBody] CreateProposalRequestDto proposalDto)
        {
            var result = await _proposalService.AddAsync(proposalDto, User);
            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            return CreatedAtAction(nameof(GetById), new {id = result.Data.Id}, result.Data.ToProposalDto());
        }

        /// <summary>
        ///  Creates a counter-proposal for an existing proposal.
        /// </summary>
        /// <param name="proposalId">The unique identifier of the proposal to counter.</param>
        /// <param name="counterProposalDto">The data transfer object containing the counter proposal details.</param>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /api/proposals/1
        ///     {
        ///         "comment": "This is a counter proposal comment",
        ///         "paymentRatios": [
        ///             {
        ///                 "partyId": 1,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 50
        ///             },
        ///             {
        ///                 "partyId": 2,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 50
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Returns the created counter proposal.</returns>
        /// /// <response code="201">Successfully created a new counter-proposal.</response>
        /// <response code="400">Invalid request data or validation errors.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks necessary permissions.</response>
        /// <response code="404">The original proposal does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPost("{proposalId:int}")]
        [ProducesResponseType(typeof(ProposalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> AddCounterProposal([FromRoute] int proposalId, [FromBody] CreateCounterProposalRequestDto counterProposalDto)
        {
            var result = await _proposalService.AddCounterProposalAsync(proposalId, counterProposalDto, User);
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new { Errors = result.Errors })
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data.ToProposalDto());
        }

        /// <summary>
        /// Reviews a proposal by accepting or rejecting it with counter proposal.
        /// </summary>
        /// <param name="proposalId">The unique identifier of the proposal to review.</param>
        /// <param name="reviewProposalDto">The data transfer object containing the review details.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/proposals/1/payment-ratios
        ///     {
        ///         "response": "Accepted"
        ///     }
        ///     
        ///     OR
        ///     
        ///     POST /api/proposals/1/payement-ratios
        ///     {
        ///         "response": "Rejected",
        ///         "comment": "I propose a different payment structure.",
        ///         "paymentRatios": [
        ///             {
        ///                 "partyId": 1,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 70
        ///             },
        ///             {
        ///                 "partyId": 2,
        ///                 "paymentType": "Percentage",
        ///                 "paymentAmount": 30
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Proposal successfully reviewed.</response>
        /// <response code="400">Invalid request data or validation errors.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not allowed to review this proposal.</response>
        /// <response code="404">The proposal does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPost("{proposalId:int}/payment-ratios")]
        [ProducesResponseType(typeof(ProposalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> ReviewProposal([FromRoute] int proposalId, [FromBody] ReviewProposalDto reviewProposalDto)
        {
            var result = await _proposalService.ReviewProposalAsync(proposalId, reviewProposalDto, User);
            if(!result.IsSuccess)
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
        /// Updates an existing proposal.
        /// </summary>
        /// <param name="id">The unique identifier of the proposal to update.</param>
        /// <param name="updateProposalDto">The data transfer object containing updated proposal details.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/proposals/1
        ///     {
        ///         "comment": "Updated comment"
        ///     }
        /// 
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Proposal successfully updated.</response>
        /// <response code="400">Invalid request data or validation errors.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not allowed to update this proposal.</response>
        /// <response code="404">The proposal does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProposalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProposalRequestDto updateProposalDto)
        {
            var result = await _proposalService.UpdateAsync(id, updateProposalDto);
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

        /*[HttpPut("{proposalId:int}/review")]
        public async Task<IActionResult> EvaluateProposal([FromRoute] int proposalId, [FromBody] EvaluateProposalDto evaluateProposalDto)
        {
            var result = await _proposalService.EvaluateProposalAsync(proposalId, evaluateProposalDto, User);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new { Errors = result.Errors })
                };
            }

            return Ok(result.Data);
        }*/
        /*
        [HttpPut("{proposalId:int}/accept")]
        public async Task<IActionResult> AcceptProposal([FromRoute] int proposalId)
        {
            var result = await _proposalService.AcceptProposalAsync(proposalId, User);
            if(!result.IsSuccess)
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

        [HttpPost("{proposalId:int}/reject")]
        public async Task<IActionResult> RejectProposal([FromRoute] int proposalId, [FromBody] CreateCounterProposalRequestDto counterProposalDto)
        {
            var result = await _proposalService.RejectProposalAsync(proposalId, counterProposalDto, User);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new { Errors = result.Errors })
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data.ToProposalDto());
        }
        */

        /// <summary>
        /// Deletes an existing proposal by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the proposal to delete.</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/proposals/1
        /// 
        /// </remarks>
        /// <returns>Returns no content.</returns>
        /// <response code="204">Proposal successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User lacks neccessary permissions to delete this proposal.</response>
        /// <response code="404">The proposal does not exist.</response>
        /// <response code="500">Unexpected error while processing the request.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _proposalService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
