using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemProposalAPI.Migrations
{
    /// <inheritdoc />
    public partial class Cascade_delete_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_ItemParties_ItemId_PartyId",
                table: "ProposalItemParties");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalItemParties_ItemParties_ItemId_PartyId",
                table: "ProposalItemParties",
                columns: new[] { "ItemId", "PartyId" },
                principalTable: "ItemParties",
                principalColumns: new[] { "ItemId", "PartyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_ItemParties_ItemId_PartyId",
                table: "ProposalItemParties");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalItemParties_ItemParties_ItemId_PartyId",
                table: "ProposalItemParties",
                columns: new[] { "ItemId", "PartyId" },
                principalTable: "ItemParties",
                principalColumns: new[] { "ItemId", "PartyId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
