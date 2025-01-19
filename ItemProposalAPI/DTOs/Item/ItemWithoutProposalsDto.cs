using ItemProposalAPI.Models;

namespace ItemProposalAPI.DTOs.Item
{
    public class ItemWithoutProposalsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Creation_Date { get; set; } = DateTime.Now;
        public Status Share_Status { get; set; }
    }
}
