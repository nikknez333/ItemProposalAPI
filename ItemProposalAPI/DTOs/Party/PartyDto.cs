using System.ComponentModel.DataAnnotations;

namespace ItemProposalAPI.DTOs.Party
{
    public class PartyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
