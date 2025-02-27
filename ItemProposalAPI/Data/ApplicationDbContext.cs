using ItemProposalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ItemProposalAPI.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
          
        }

        //Database tables that can be accessed through ApplicationDbContext
        //public DbSet<User> Users { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ItemParty> ItemParties { get; set; }
        public DbSet<ProposalItemParty> ProposalItemParties { get; set; }

        //Set up many-to-many relationships with Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //App Identity Roles
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                { 
                    Id = "UserPartyOwner",
                    Name = "UserPartyOwner",
                    NormalizedName = "USERPARTYOWNER",
                },

                new IdentityRole
                {
                    Id = "UserPartyEmployee",
                    Name = "UserPartyEmployee",
                    NormalizedName = "USERPARTYEMPLOYEE",
                },

                new IdentityRole
                {
                    Id = "UserUnemployed",
                    Name = "UserUnemployed",
                    NormalizedName = "USERUNEMPLOYED",
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            modelBuilder.Entity<ItemParty>()
                .HasKey(ip => new { ip.ItemId, ip.PartyId });
            modelBuilder.Entity<ItemParty>()
                .HasOne(ip => ip.Item)
                .WithMany(i => i.ItemParties)
                .HasForeignKey(ip => ip.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemParty>()
                .HasOne(ip => ip.Party)
                .WithMany(p => p.ItemParties)
                .HasForeignKey(ip => ip.PartyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProposalItemParty>()
                .HasKey(pip => new { pip.ProposalId, pip.ItemId, pip.PartyId });
            modelBuilder.Entity<ProposalItemParty>()
                .HasOne(pip => pip.Proposal)
                .WithMany(p => p.ProposalItemParties)
                .HasForeignKey(pip => pip.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProposalItemParty>()
                .HasOne(pip => pip.ItemParty)
                .WithMany(ip => ip.ProposalItemParties)
                .HasForeignKey(pip => new { pip.ItemId, pip.PartyId })
                .OnDelete(DeleteBehavior.ClientCascade);

            //recursive relationship between Proposal and Counter Proposal
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.InitialProposal)
                .WithMany(p => p.CounterProposals)
                .HasForeignKey(p => p.CounterToProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            //Proposal constraint -> comment is optional for Initial Proposal, but mandatory for Counter Proposal
            modelBuilder.Entity<Proposal>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Proposal_Comment_Required",
                    "CounterToProposalId is NULL OR LEN(Comment) > 0"
                ));

            //value conversions for enums to string
            modelBuilder.Entity<Item>()
                .Property(i => i.Share_Status)
                .HasConversion<string>();

            modelBuilder.Entity<Proposal>()
                .Property(p => p.Proposal_Status)
                .HasConversion<string>();

            modelBuilder.Entity<ProposalItemParty>()
                .Property(pip => pip.PaymentType)
                .HasConversion<string>();
            
            modelBuilder.Entity<ProposalItemParty>()
                .Property(pip => pip.Response)
                .HasConversion<string>();

            modelBuilder.Entity<ProposalItemParty>()
            .Property(p => p.PaymentAmount)
            .HasColumnType("decimal(18,2)");

            //Add indexes to frequently queried data to boost query performance
        }
        
        public static async Task Seed(ApplicationDbContext context, UserManager<User> userManager)
        {
            if(!context.Parties.Any())
            {
                var parties = new List<string>
                {
                    "Apple",
                    "Google",
                    "Facebook",
                    "Microsoft",
                    "Open AI"
                };

                foreach(var partyName in parties)
                {
                    var company = new Party
                    {
                        Name = partyName,
                    };

                    await context.Parties.AddAsync(company);
                    await context.SaveChangesAsync();
                }
            }


            if(!context.Users.Any())
            {               
                var partyOwners = new List<(string username, string password,int partyId)>
                {
                    ("timCook3", "CoO3im!TIM", 1),
                    ("sPichai881", "piCHAI99?8", 2),
                    ("mArkZuck23", "zuCkie?226", 3),
                    ("saTYaNadl115", "4_TYnadALL", 4),
                    ("sAmAlt331", "AltMAN99!?", 5)
                };

                foreach(var partyOwner in partyOwners)
                {
                    var user = new User
                    {
                        UserName = partyOwner.username,
                        PartyId = partyOwner.partyId,
                    };

                    var result = await userManager.CreateAsync(user, partyOwner.password);
                    if(result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "UserPartyOwner");
                    }
                }

                await context.SaveChangesAsync();
            }

            /*
            if (context.Users.Any())
                return; // Exit if already seeded

            // Seed Parties (Generate 5000 parties)
            var parties = new List<Party>();
            for (int i = 0; i < 5000; i++)
            {
                parties.Add(new Party { Name = $"Party {i + 1}" });
            }
            context.Parties.AddRange(parties);
            context.SaveChanges();  // Save after parties are added

            // Seed Users (Generate 10000 users)
            var users = new List<User>();
            for (int i = 0; i < 10000; i++)
            {
                users.Add(new User
                {
                    Username = $"user{i + 1}",
                    PartyId = parties[i % parties.Count].Id  // Assign users randomly to parties
                });
            }
            context.Users.AddRange(users);
            context.SaveChanges();  // Save after users are added

            // Seed Items (Generate 2000 items)
            var items = new List<Item>();
            for (int i = 0; i < 2000; i++)
            {
                items.Add(new Item
                {
                    Name = $"Item {i + 1}",
                    Share_Status = (Status)(i % 2)  // Alternate between Not_Shared and Shared
                });
            }
            context.Items.AddRange(items);
            context.SaveChanges();  // Save after items are added

            // Seed Proposals (Generate 50000 proposals)
            var proposals = new List<Proposal>();
            for (int i = 0; i < 50000; i++)
            {
                proposals.Add(new Proposal
                {
                    UserId = users[i % users.Count].Id,  // Assign users to proposals
                    ItemId = items[i % items.Count].Id,  // Assign items to proposals
                    Proposal_Status = (Proposal_Status)(i % 3),  // Randomize proposal status
                    Comment = $"Proposal {i + 1} for {items[i % items.Count].Name}",
                    Created_At = DateTime.UtcNow.AddMinutes(-i)  // Simulate older proposals
                });
            }
            context.Proposal.AddRange(proposals);
            context.SaveChanges();  // Save after proposals are added

            // Seed ItemParties (Many-to-many relationship, generate 10000)
            var itemParties = new List<ItemParty>();
            for (int i = 0; i < 10000; i++)
            {
                itemParties.Add(new ItemParty
                {
                    ItemId = items[i % items.Count].Id,
                    PartyId = parties[i % parties.Count].Id
                });
            }
            context.ItemParties.AddRange(itemParties);
            context.SaveChanges();  // Save after itemParties are added

            // Seed ProposalItemParties (Many-to-many relationship, generate 50000)
            var proposalItemParties = new List<ProposalItemParty>();
            for (int i = 0; i < 50000; i++)
            {
                proposalItemParties.Add(new ProposalItemParty
                {
                    ProposalId = proposals[i % proposals.Count].Id,
                    ItemId = items[i % items.Count].Id,
                    PartyId = parties[i % parties.Count].Id,
                    PaymentType = (PaymentType)(i % 2),  // Alternate between Percentage and Fixed
                    PaymentAmount = (decimal)(i % 1000)  // Simulate various payment amounts
                });
            }
            context.ProposalItemParties.AddRange(proposalItemParties);
            context.SaveChanges();  // Save after proposalItemParties are added
            */
        }
        
    }
}
