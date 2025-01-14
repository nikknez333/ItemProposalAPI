using ItemProposalAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.DTOs.Item
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Creation_Date { get; set; } = DateTime.Now;
        public Status Share_Status { get; set; }
    }
}
