using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Identity.Client;

namespace ItemProposalAPI.Mappers
{
    public static class ItemMapper
    {
        public static ItemDto ToItemDto(this Item itemModel)
        {
            return new ItemDto
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Creation_Date = itemModel.Creation_Date,
                Share_Status = itemModel.Share_Status,
            };
        }

        public static Item ToItemFromCreateDto(this CreateItemRequestDto createItemDto)
        {
            return new Item
            {
                Name = createItemDto.Name,
                Creation_Date = createItemDto.Creation_Date,
                Share_Status = createItemDto.Share_Status
            };
        }

        public static Item ToItemFromUpdateDto(this Item item, UpdateItemRequestDto updateItemDto)
        {
            item.Name = updateItemDto.Name;
            item.Creation_Date = updateItemDto.Creation_Date;

            return item;
        }
    }
}
