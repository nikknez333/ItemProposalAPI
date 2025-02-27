using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemProposalAPI.Migrations
{
    /// <inheritdoc />
    public partial class Cascade_delete_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_AspNetUsers_UserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Items_ItemId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Proposal_CounterToProposalId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_Proposal_ProposalId",
                table: "ProposalItemParties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal");

            migrationBuilder.RenameTable(
                name: "Proposal",
                newName: "Proposals");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_UserId",
                table: "Proposals",
                newName: "IX_Proposals_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_ItemId",
                table: "Proposals",
                newName: "IX_Proposals_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_CounterToProposalId",
                table: "Proposals",
                newName: "IX_Proposals_CounterToProposalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalItemParties_Proposals_ProposalId",
                table: "ProposalItemParties",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_UserId",
                table: "Proposals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Items_ItemId",
                table: "Proposals",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Proposals_CounterToProposalId",
                table: "Proposals",
                column: "CounterToProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_Proposals_ProposalId",
                table: "ProposalItemParties");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_UserId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Items_ItemId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Proposals_CounterToProposalId",
                table: "Proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals");

            migrationBuilder.RenameTable(
                name: "Proposals",
                newName: "Proposal");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_UserId",
                table: "Proposal",
                newName: "IX_Proposal_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ItemId",
                table: "Proposal",
                newName: "IX_Proposal_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_CounterToProposalId",
                table: "Proposal",
                newName: "IX_Proposal_CounterToProposalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_AspNetUsers_UserId",
                table: "Proposal",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Items_ItemId",
                table: "Proposal",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Proposal_CounterToProposalId",
                table: "Proposal",
                column: "CounterToProposalId",
                principalTable: "Proposal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalItemParties_Proposal_ProposalId",
                table: "ProposalItemParties",
                column: "ProposalId",
                principalTable: "Proposal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
