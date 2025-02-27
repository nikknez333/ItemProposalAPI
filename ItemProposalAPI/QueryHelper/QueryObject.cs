using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemProposalAPI.QueryHelper
{
    public class QueryObject
    {
        private const int maxPageSize = 100;
        private int _pageSize = 10;

        //Filtering
        public string? Name { get; set; } = null;
        public DateTime? From_Creation_Date { get; set; } = null;
        public DateTime? To_Creation_Date { get; set; } = null;
        public Status? Share_Status { get; set; } = null;
        //Sorting
        public SortOptions SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }

    public enum SortOptions
    {
        Name = 0,
        Creation_Date,
        Share_Status
    }
}
