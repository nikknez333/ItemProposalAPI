using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemProposalAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Creation_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Share_Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemParties",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemParties", x => new { x.ItemId, x.PartyId });
                    table.ForeignKey(
                        name: "FK_ItemParties_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemParties_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartyId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Proposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Proposal_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CounterToProposalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposal", x => x.Id);
                    table.CheckConstraint("CK_Proposal_Comment_Required", "CounterToProposalId is NULL OR LEN(Comment) > 0");
                    table.ForeignKey(
                        name: "FK_Proposal_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposal_Proposal_CounterToProposalId",
                        column: x => x.CounterToProposalId,
                        principalTable: "Proposal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposal_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposalItemParties",
                columns: table => new
                {
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalItemParties", x => new { x.ProposalId, x.ItemId, x.PartyId });
                    table.ForeignKey(
                        name: "FK_ProposalItemParties_ItemParties_ItemId_PartyId",
                        columns: x => new { x.ItemId, x.PartyId },
                        principalTable: "ItemParties",
                        principalColumns: new[] { "ItemId", "PartyId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProposalItemParties_Proposal_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemParties_PartyId",
                table: "ItemParties",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_CounterToProposalId",
                table: "Proposal",
                column: "CounterToProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_ItemId",
                table: "Proposal",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_UserId",
                table: "Proposal",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalItemParties_ItemId_PartyId",
                table: "ProposalItemParties",
                columns: new[] { "ItemId", "PartyId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PartyId",
                table: "Users",
                column: "PartyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalItemParties");

            migrationBuilder.DropTable(
                name: "ItemParties");

            migrationBuilder.DropTable(
                name: "Proposal");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
