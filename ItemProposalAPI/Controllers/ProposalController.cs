using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace ItemProposalAPI.Controllers
{
    [Route("api/proposals")]
    [ApiController]
    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;

        public ProposalController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(/*[FromQuery] QueryObject queryObject*/)
        {
            var result = await _proposalService.GetAllAsync();
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
            var result = await _proposalService.AddAsync(proposalDto);
            if (!result.IsSuccess)
                return BadRequest(new
                {
                    Errors = result.Errors
                });

            return CreatedAtAction(nameof(GetById), new {id = result.Data.Id}, result.Data.ToProposalDto());
        }

        [HttpPost("{proposalId:int}/counter")]
        public async Task<IActionResult> AddCounterProposal([FromRoute] int proposalId, [FromBody] CreateCounterProposalRequestDto counterProposalDto)
        {
            var result = await _proposalService.AddCounterProposalAsync(proposalId, counterProposalDto);
            if (!result.IsSuccess)
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result.Errors),
                    ErrorType.BadRequest => BadRequest(result.Errors),
                    _ => StatusCode(500, new { Errors = result.Errors })
                };

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data.ToProposalDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProposalRequestDto proposalDto)
        {
            var result = await _proposalService.UpdateAsync(id, proposalDto);
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
