using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ItemProposalAPI.Controllers
{
    [Route("api/proposals")]
    [ApiController]
    public class ProposalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProposalController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proposals = await _unitOfWork.ProposalRepository.GetAllAsync();

            var proposalDTOs = proposals.Select(p => p.ToProposalDto());

            return Ok(proposalDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var proposal = await _unitOfWork.ProposalRepository.GetByIdAsync(id);
            if (proposal == null)
                return NotFound($"Proposal with id:{id} does not exist.");

            return Ok(proposal.ToProposalDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProposalRequestDto proposalDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(proposalDto.UserId);
            if (user == null)
                return BadRequest($"User with Id:{proposalDto.UserId} does not exist");

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(proposalDto.ItemId);
            if (item == null)
                return BadRequest($"Item with Id:{proposalDto.ItemId} does not exist");

            if (user.PartyId == null)
                return BadRequest($"Users that are not part of any party can't make proposals.");
            
            //check is item status is shared
            if (item.Share_Status == Models.Status.Not_Shared)
                return BadRequest($"Proposal for item with ID: {item.Id} can't be made, because item is not shared");

            var proposalModel = proposalDto.ToProposalFromCreateDto();

            await _unitOfWork.ProposalRepository.AddAsync(proposalModel);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new {id = proposalModel.Id}, proposalModel.ToProposalDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProposalRequestDto proposalDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var existingProposal = await _unitOfWork.ProposalRepository.GetByIdAsync(id);
            if (existingProposal == null)
                return NotFound($"Proposal with id:{id} does not exist.");

            _unitOfWork.ProposalRepository.UpdateAsync(proposalDto.ToProposalFromUpdateDto(existingProposal));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(existingProposal.ToProposalDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedProposal = await _unitOfWork.ProposalRepository.DeleteAsync(id);
            if(deletedProposal == null)
                return NotFound($"Proposal with id:{id} does not exist.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
