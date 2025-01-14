using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;

namespace ItemProposalAPI.Repository.Repositories
{
    public class UserRepository : RepositoryGeneric<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
