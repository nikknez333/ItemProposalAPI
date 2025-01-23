using ItemProposalAPI.DTOs.ProposalItemParty;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.Models;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/proposal-item-parties")]
    [ApiController]
    public class ProposalItemPartyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProposalItemPartyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proposalItemParties = await _unitOfWork.ProposalItemPartyRepository.GetAllAsync();

            var proposalItemPartyDTOs = proposalItemParties.Select(pip => pip.ToProposalItemPartyDto());

            return Ok(proposalItemPartyDTOs);
        }

        [HttpGet("{proposalId}/{itemId}/{partyId}")]
        public async Task<IActionResult> GetById([FromRoute] int proposalId, [FromRoute] int itemId, [FromRoute] int partyId)
        {
            var proposalItemParty = await _unitOfWork.ProposalItemPartyRepository.GetByIdAsync(proposalId, itemId, partyId);
            if (proposalItemParty == null)
                return NotFound($"Record with ids:{proposalId}/{itemId}/{partyId} is not found");

            return Ok(proposalItemParty.ToProposalItemPartyDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProposalItemPartyRequestDto proposalItemPartyDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            //check does proposal exist in proposal table to ensure consistency with proposalItemParty table
            var proposal = await _unitOfWork.ProposalRepository.GetByIdAsync(proposalItemPartyDto.ProposalId);
            if (proposal == null)
                return BadRequest($"Proposal with id:{proposalItemPartyDto.ProposalId} does not exist.");

            //get all parties involved in sharing the item so that payment ratio proposed to that party can be added
            var involvedParties = await _unitOfWork.ItemPartyRepository.GetPartiesSharingItemAsync(proposalItemPartyDto.ItemId);
            //validation: get all partyIds from HTTP POST body to make sure request is valid
            var providedPartyIds = proposalItemPartyDto.PaymentRatios.Select(pr => pr.PartyId).ToList();
            //if HTTP POST body does not have exact number of payment ratios proposed for all involved parties return BadRequest
            if (providedPartyIds.Count != involvedParties.Count())
            {
                return BadRequest($"All {involvedParties.Count()} parties involved in sharing the item must be included in the request.");
            }
            //if HTTP POST body have duplicate payment ratios, in other words multiple payment ratios proposed for same party involved in sharing the item
            var partyIdCount = proposalItemPartyDto.PaymentRatios
                .GroupBy(pip => pip.PartyId)
                .Where(g => g.Count() > 1)
                .ToList();

            if (partyIdCount.Any())
                return BadRequest($"Duplicate payment ratios found for PartyId(s): {string.Join(", ", partyIdCount.Select(g => g.Key))}");

            foreach (var party in involvedParties)
            {
                if (!providedPartyIds.Contains(party.Id))
                {
                    return BadRequest($"Missing payment ratio for Party with Id {party.Id}");
                }
            }

            var createdUris = new List<object>();
            foreach (var ratio in proposalItemPartyDto.PaymentRatios)
            {
                var proposalItemParty = new ProposalItemParty
                {
                    ProposalId = proposalItemPartyDto.ProposalId,
                    ItemId = proposalItemPartyDto.ItemId,
                    PartyId = ratio.PartyId,
                    PaymentType = ratio.PaymentType,
                    PaymentAmount = ratio.PaymentAmount,
                };

                await _unitOfWork.ProposalItemPartyRepository.AddAsync(proposalItemParty);

                createdUris.Add(Url.Action(nameof(GetById), new
                {
                    proposalId = proposalItemPartyDto.ProposalId,
                    itemId = proposalItemPartyDto.ItemId,
                    partyId = ratio.PartyId
                }));

            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Created(string.Empty, createdUris);
        }

        [HttpDelete("{proposalId}/{partyId}/{itemId}")]
        public async Task<IActionResult> Delete([FromRoute] int proposalId, [FromRoute] int partyId, [FromRoute] int itemId)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedProposalItemParty = await _unitOfWork.ProposalItemPartyRepository.DeleteAsync(proposalId, itemId, partyId);
            if(deletedProposalItemParty == null) 
                return NotFound($"Record with Proposal ID:{proposalId} Party ID: {partyId} Item ID: {itemId} is not found.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
