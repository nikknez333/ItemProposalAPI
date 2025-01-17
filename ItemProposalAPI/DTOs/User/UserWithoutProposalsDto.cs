namespace ItemProposalAPI.DTOs.User
{
    public class UserWithoutProposalsDto
    {
        public int Id { get; set; }
        public int? PartyId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
