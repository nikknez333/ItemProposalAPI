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

        [HttpGet]
        [Authorize(Roles = "UserPartyOwner,UserPartyEmployee")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _itemPartyService.GetAllAsync(pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{partyId:int}/{itemId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int partyId, [FromRoute] int itemId)
        {
            var result = await _itemPartyService.GetByIdAsync(partyId, itemId);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost]
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

            return CreatedAtAction(nameof(GetById), new {result.Data.PartyId,  result.Data.ItemId}, result.Data);
        }

        [HttpDelete("{partyId:int}/{itemId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int partyId, [FromRoute] int itemId)
        {
            var result = await _itemPartyService.DeleteAsync(partyId, itemId);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
