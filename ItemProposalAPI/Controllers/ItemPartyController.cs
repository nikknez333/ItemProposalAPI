using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/item-parties")]
    [ApiController]
    public class ItemPartyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemPartyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itemParties = await _unitOfWork.ItemPartyRepository.GetAllAsync(ip => ip.Party, ip => ip.Item);

            var itemPartyDTOs = itemParties.Select(ip => ip.ToItemPartyDto());

            return Ok(itemPartyDTOs);  
        }

        [HttpGet("{partyId}/{itemId}")]
        public async Task<IActionResult> GetById([FromRoute] int partyId, [FromRoute] int itemId)
        {
            var itemParty = await _unitOfWork.ItemPartyRepository.GetByIdAsync(partyId, itemId, ip => ip.Party, ip => ip.Item);
            if (itemParty == null)
                return NotFound($"ItemParty with party ID:{partyId} / item ID: {itemId} does not exist.");

            return Ok(itemParty.ToItemPartyDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateItemPartyRequestDto itemPartyDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemPartyDto.ItemId);
            if (item == null)
                return BadRequest($"Item with id:{itemPartyDto.ItemId} does not exist.");

            var party = await _unitOfWork.PartyRepository.GetByIdAsync(itemPartyDto.PartyId);
            if (party == null)
                return BadRequest($"Party with id:{itemPartyDto.PartyId} does not exist.");

            var itemPartyModel = itemPartyDto.ToItemPartyFromCreateDto();

            await _unitOfWork.ItemPartyRepository.AddItemPartyAsync(itemPartyModel);

            await _unitOfWork.SaveChangesAsync();
            //update-Shared-status if neccessary
            var parties = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(itemPartyDto.ItemId);
            if(parties.Count() > 1)
            {
                var itemModel = await _unitOfWork.ItemRepository.GetByIdAsync(itemPartyDto.ItemId);
                itemModel.Share_Status = Status.Shared;
                _unitOfWork.ItemRepository.UpdateAsync(itemModel);
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new {itemPartyDto.PartyId,  itemPartyDto.ItemId}, itemPartyModel.ToItemPartyDto());
        }

        [HttpDelete("{partyId}/{itemId}")]
        public async Task<IActionResult> Delete([FromRoute] int partyId, [FromRoute] int itemId)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedItemParty = await _unitOfWork.ItemPartyRepository.RemoveItemPartyAsync(partyId, itemId);
            if (deletedItemParty == null)
                return NotFound($"ItemParty with party ID:{partyId} / item ID: {itemId} does not exist.");

            await _unitOfWork.SaveChangesAsync();

            var parties = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(itemId);
            if (parties.Count() <= 1)
            {
                var itemModel = await _unitOfWork.ItemRepository.GetByIdAsync(itemId);
                itemModel.Share_Status = Status.Not_Shared;
                _unitOfWork.ItemRepository.UpdateAsync(itemModel);
            }
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
