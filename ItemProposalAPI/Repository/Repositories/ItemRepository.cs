using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Models;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using Microsoft.EntityFrameworkCore;

namespace ItemProposalAPI.Repository.Repositories
{
    public class ItemRepository : RepositoryGeneric<Item, int>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
