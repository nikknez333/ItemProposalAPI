using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.Proposals!);

            var userDTOs = users.Select(u => u.ToUserDto());

            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id, u => u.Proposals!);

            if (user == null)
                return NotFound($"User with id:{id} does not exist.");

            return Ok(user.ToUserDto());
        }

        [HttpGet("{userId}/party/items")]
        public async Task<IActionResult> GetAllMyPartyItems([FromRoute] int userId, [FromQuery] QueryObject query)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound($"User with id:{userId} does not exist.");

            if (user.PartyId == null)
                return NotFound($"User with id:{userId} is not associated with any party");

            var partyItems = await _unitOfWork.ItemPartyRepository.GetPartyItemsAsync(user.PartyId, query);
            if (partyItems == null)
                return NotFound($"Party does not own shares of any item.");

            var partyItemDTOs = partyItems.Select(p => p.ToItemWithoutProposalsDto());

            return Ok(partyItemDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserRequestDto userDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var userModel = userDto.ToUserFromCreateDto();
            //if HTTP POST want to create user who is part of some company
            if (userModel.PartyId.HasValue)
            {
                //check does that party exist
                var party = await _unitOfWork.PartyRepository.GetByIdAsync((int)userModel.PartyId);
                if (party == null)
                    return BadRequest($"Party with id:{userModel.PartyId} does not exist.");
            }

            await _unitOfWork.UserRepository.AddAsync(userModel);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id =  userModel.Id }, userModel.ToUserDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound($"User with id:{id} does not exist.");

            if (userDto.PartyId.HasValue)
            {
                //check does that party exist
                var party = await _unitOfWork.PartyRepository.GetByIdAsync((int)userDto.PartyId);
                if (party == null)
                    return BadRequest($"Party with id:{userDto.PartyId} does not exist.");
            }

            _unitOfWork.UserRepository.UpdateAsync(userDto.ToUserFromUpdateDto(existingUser));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(existingUser.ToUserDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedUser = await _unitOfWork.UserRepository.DeleteAsync(id);
            if(deletedUser == null)
                return NotFound($"User with id:{id} does not exist.");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
