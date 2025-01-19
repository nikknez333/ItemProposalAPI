using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemProposalAPI.QueryHelper
{
    public class QueryObject
    {
        //Filtering
        public string? Name { get; set; } = null;
        public DateTime? From_Creation_Date { get; set; } = null;
        public DateTime? To_Creation_Date { get; set; } = null;
        public Status? Share_Status { get; set; } = null;
        //Sorting
        public ItemWithoutProposalsDto? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}
