using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemProposalAPI.Migrations
{
    /// <inheritdoc />
    public partial class Cascade_delete_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Proposal_CounterToProposalId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_Proposal_ProposalId",
                table: "ProposalItemParties");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Proposal_CounterToProposalId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalItemParties_Proposal_ProposalId",
                table: "ProposalItemParties");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Proposal_CounterToProposalId",
                table: "Proposal",
                column: "CounterToProposalId",
                principalTable: "Proposal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalItemParties_Proposal_ProposalId",
                table: "ProposalItemParties",
                column: "ProposalId",
                principalTable: "Proposal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
