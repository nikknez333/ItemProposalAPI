using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public int? PartyId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
