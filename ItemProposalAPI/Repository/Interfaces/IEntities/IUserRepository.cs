using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IRepositoryGeneric;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ItemProposalAPI.Repository.Interfaces.IEntities
{
    public interface IUserRepository : IRepositoryGeneric<User, int>
    {
    }
}
