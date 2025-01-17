using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
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
                return NotFound();

            return Ok(user.ToUserDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserRequestDto userDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var userModel = userDto.ToUserFromCreateDto();

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
                return NotFound();

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
                return NotFound();

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
