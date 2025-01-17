namespace ItemProposalAPI.DTOs.User
{
    public class UpdateUserRequestDto
    {
        public int? PartyId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
