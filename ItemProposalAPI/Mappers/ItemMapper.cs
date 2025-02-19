using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

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
                Proposals = itemModel.Proposals!.Select(p => p.ToProposalDto()).ToList()
            };
        }

        public static ItemNegotiationDto ToItemNegotiationDto(this Item itemModel, IEnumerable<Proposal> proposals, User user)
        {
            return new ItemNegotiationDto
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Creation_Date = itemModel.Creation_Date,
                Share_Status = itemModel.Share_Status,
                Proposals = proposals.Select(p => p.ToProposalNegotiationDto(user)).ToList()
            };
        }

        public static ItemWithoutProposalsDto ToItemWithoutProposalsDto(this Item itemModel)
        {
            return new ItemWithoutProposalsDto
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
            };
        }

        public static Item ToItemFromUpdateDto(this UpdateItemRequestDto updateItemDto, Item item)
        {
            item.Name = updateItemDto.Name;

            return item;
        }
    }
}
