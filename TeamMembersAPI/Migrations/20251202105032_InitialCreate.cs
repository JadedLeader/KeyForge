using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamMembersAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorisationLevel = table.Column<int>(type: "int", nullable: false),
                    AccountCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamAcceptingInvites = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MemberCap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Team_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamVault",
                columns: table => new
                {
                    TeamVaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamVaultDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamVaultName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastTeamUpdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamVault", x => x.TeamVaultId);
                    table.ForeignKey(
                        name: "FK_TeamVault_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamInvite",
                columns: table => new
                {
                    TeamInviteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamVaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviteSentBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InviteRecipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InviteStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InviteCreatedAt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamInvite", x => x.TeamInviteId);
                    table.ForeignKey(
                        name: "FK_TeamInvite_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamInvite_TeamVault_TeamVaultId",
                        column: x => x.TeamVaultId,
                        principalTable: "TeamVault",
                        principalColumn: "TeamVaultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamVaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleAccess = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.TeamMemberId);
                    table.ForeignKey(
                        name: "FK_TeamMember_TeamVault_TeamVaultId",
                        column: x => x.TeamVaultId,
                        principalTable: "TeamVault",
                        principalColumn: "TeamVaultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Team_AccountId",
                table: "Team",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInvite_AccountId",
                table: "TeamInvite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInvite_TeamVaultId",
                table: "TeamInvite",
                column: "TeamVaultId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamVaultId",
                table: "TeamMember",
                column: "TeamVaultId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamVault_TeamId",
                table: "TeamVault",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamInvite");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "TeamVault");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
