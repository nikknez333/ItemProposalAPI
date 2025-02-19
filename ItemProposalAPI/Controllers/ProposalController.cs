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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _proposalService.GetAllAsync(pagination);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _proposalService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost]
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

        [HttpPost("{proposalId:int}")]
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

        [HttpPost("{proposalId:int}/payment-ratios")]
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

        [HttpPut("{id:int}")]
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
