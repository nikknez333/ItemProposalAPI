using ItemProposalAPI.DataAccess;
using ItemProposalAPI.Repository.Interfaces.IEntities;
using ItemProposalAPI.Repository.Repositories;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace ItemProposalAPI.UnitOfWorkPattern.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private IUserRepository _userRepository;
        private IPartyRepository _partyRepository;
        private IItemRepository _itemRepository;
        private IProposalRepository _proposalRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUserRepository UserRepository 
        { 
            get
            {
                return _userRepository = _userRepository ?? new UserRepository(_dbContext);
            }
        }

        public IPartyRepository PartyRepository 
        { 
            get
            {
                return _partyRepository = _partyRepository ?? new PartyRepository(_dbContext);
            }
        }

        public IItemRepository ItemRepository 
        { 
            get
            {
                return _itemRepository = _itemRepository ?? new ItemRepository(_dbContext);
            }
        }

        public IProposalRepository ProposalRepository 
        { 
            get
            {
                return _proposalRepository = _proposalRepository ?? new ProposalRepository(_dbContext);
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing) 
                {
                    _dbContext.Dispose();
                }
            }

            this._disposed = true;
        }
    }
}
