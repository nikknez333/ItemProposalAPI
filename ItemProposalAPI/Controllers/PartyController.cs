using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/parties")]
    [ApiController]
    public class PartyController : ControllerBase
    {
        private readonly IPartyService _partyService;

        public PartyController(IPartyService partyService)
        {
            _partyService = partyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(/*[FromQuery] QueryObject queryObject*/)
        {
            var result = await _partyService.GetAllAsync();
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _partyService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{partyId:int}/items")]
        public async Task<IActionResult> GetPartyItems([FromRoute] int partyId)
        {
            var result = await _partyService.GetPartyItemsAsync(partyId);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreatePartyRequestDto createPartyDto)
        {
            var result = await _partyService.AddAsync(createPartyDto);
            if (!result.IsSuccess)
                return BadRequest(new
                {
                    Errors = result.Errors
                });

            return CreatedAtAction(nameof(GetById), new {id =  result.Data.Id}, result.Data.ToPartyWithoutUsersDto());
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _partyService.DeleteAsync(id);
            if(!result.IsSuccess)
                return NotFound(result.Errors); 

            return NoContent();
        }

    }
}
