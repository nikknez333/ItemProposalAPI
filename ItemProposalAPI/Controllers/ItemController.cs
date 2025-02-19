using ItemProposalAPI.DTOs.Item;
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationObject pagination)
        {
            var result = await _itemService.GetAllAsync(pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _itemService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{itemId:int}/parties")]
        public async Task<IActionResult> GetPartiesSharingItem([FromRoute] int itemId, [FromQuery] PaginationObject pagination)
        {
            var result = await _itemService.GetPartiesSharingItemAsync(itemId, pagination);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}/proposals")]
        [Authorize(Roles = "UserPartyOwner,UserPartyEmployee")]
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

        [HttpPost]
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

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _itemService.DeleteAsync(id);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
