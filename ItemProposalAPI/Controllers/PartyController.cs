using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/parties")]
    [ApiController]
    public class PartyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PartyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var parties = await _unitOfWork.PartyRepository.GetAllAsync(p => p.Users!, p => p.ItemParties!);

            var partyDTOs = parties.Select(p => p.ToPartyDto());

            return Ok(partyDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync(id, p => p.Users!);
            if (party == null)
                return NotFound();

            return Ok(party.ToPartyDto());
        }

        [HttpGet("{partyId}/items")]
        public async Task<IActionResult> GetPartyItems([FromRoute] int partyId)
        {
            var party = await _unitOfWork.PartyRepository.GetByIdAsync(partyId);
            if (party == null)
                return BadRequest($"Party with id:{partyId} does not exist.");

            var partyitems = await _unitOfWork.ItemPartyRepository.GetPartyItems(partyId, null);
            if (partyitems == null)
                return NotFound($"Party with id:{partyId} does not own shares of any item.");

            var partyItemDTOs = partyitems.Select(i => i.ToItemWithoutProposalsDto());

            return Ok(partyItemDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreatePartyRequestDto partyDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var partyModel = partyDto.ToPartyFromCreateDto();

            await _unitOfWork.PartyRepository.AddAsync(partyModel);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new {id =  partyModel.Id}, partyModel.ToPartyDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePartyRequestDto partyDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var existingParty = await _unitOfWork.PartyRepository.GetByIdAsync(id);
            if (existingParty == null)
                return NotFound();

            _unitOfWork.PartyRepository.UpdateAsync(partyDto.ToPartyFromUpdateDto(existingParty));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(existingParty.ToPartyDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedParty = await _unitOfWork.PartyRepository.DeleteAsync(id);
            if (deletedParty == null)
                return NotFound();

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

    }
}
