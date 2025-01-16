using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.Mappers;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItemProposalAPI.Controllers
{
    [Route("api/item")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsync();

            var itemsDTOs = items.Select(i => i.ToItemDto());

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item.ToItemDto());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateItemRequestDto itemDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var itemModel = itemDto.ToItemFromCreateDto();

            await _unitOfWork.ItemRepository.AddAsync(itemModel);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetById), new {id = itemModel.Id}, itemModel.ToItemDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateItemRequestDto itemDto)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var existingItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound();

            _unitOfWork.ItemRepository.UpdateAsync(itemDto.ToItemFromUpdateDto(existingItem));

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(existingItem.ToItemDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            using var transaction = _unitOfWork.BeginTransactionAsync();

            var deletedItem = await _unitOfWork.ItemRepository.DeleteAsync(id);
            if(deletedItem == null)
              return NotFound();

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
