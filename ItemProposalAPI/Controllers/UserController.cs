using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.QueryHelper;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ItemProposalAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(/*[FromQuery] QueryObject queryObject*/)
        {
            var result = await _userService.GetAllAsync(/*queryObject*/);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);         
        }

        [HttpGet("{userId:int}/party/items")]
        public async Task<IActionResult> GetAllMyPartyItems([FromRoute] int userId, [FromQuery] QueryObject query)
        {
            var result = await _userService.GetMyPartyItemsAsync(userId, query);
            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserRequestDto createUserDto)
        {
            var result = await _userService.AddAsync(createUserDto);
            if(!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Errors = result.Errors
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data.ToUserWithoutProposalsDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);
            if(!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Errors = result.Errors }),
                    ErrorType.BadRequest => BadRequest(new { Errors = result.Errors }),
                    _ => StatusCode(500, new { Errors = result.Errors }),
                };
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _userService.DeleteAsync(id);
            if(!result.IsSuccess)
                return NotFound(result.Errors);

            return NoContent();
        }
    }
}
