using ItemProposalAPI.Models;

namespace ItemProposalAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
