using ItemProposalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemProposalAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
          
        }

        //Database tables that can be accessed through ApplicationDbContext
        public DbSet<User> Users { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Proposal> Proposal { get; set; }
        public DbSet<ItemParty> ItemParties { get; set; }
        public DbSet<ProposalItemParty> ProposalItemParties { get; set; }

        //Set up many-to-many relationships with Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemParty>()
                .HasKey(ip => new { ip.ItemId, ip.PartyId });
            modelBuilder.Entity<ItemParty>()
                .HasOne(ip => ip.Item)
                .WithMany(i => i.ItemParties)
                .HasForeignKey(ip => ip.ItemId);
            modelBuilder.Entity<ItemParty>()
                .HasOne(ip => ip.Party)
                .WithMany(p => p.ItemParties)
                .HasForeignKey(ip => ip.PartyId);

            modelBuilder.Entity<ProposalItemParty>()
                .HasKey(pip => new { pip.ProposalId, pip.ItemId, pip.PartyId });
            modelBuilder.Entity<ProposalItemParty>()
                .HasOne(pip => pip.Proposal)
                .WithMany(p => p.ProposalItemParties)
                .HasForeignKey(pip => pip.ProposalId);
            modelBuilder.Entity<ProposalItemParty>()
                .HasOne(pip => pip.ItemParty)
                .WithMany(ip => ip.ProposalItemParties)
                .HasForeignKey(pip => new { pip.ItemId, pip.PartyId });

            //recursive relationship between Proposal and Counter Proposal
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.InitialProposal)
                .WithMany(p => p.CounterProposals)
                .HasForeignKey(p => p.CounterToProposalId);

            //Proposal constraint -> comment is optional for Initial Proposal, but mandatory for Counter Proposal
            modelBuilder.Entity<Proposal>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Proposal_Comment_Required",
                    "CounterToProposalId is NULL OR LEN(Comment) > 0"
                ));

        }
    }
}
