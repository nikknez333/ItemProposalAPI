using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/proposal")]
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

            return Ok(proposals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var proposal = await _unitOfWork.ProposalRepository.GetByIdAsync(id);
            if (proposal == null)
                return NotFound();

            return Ok(proposal.ToProposalDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProposalRequestDto proposalDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

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
                return NotFound();

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
                return NotFound();

            return NoContent();
        }
    }
}
