using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class intialCreate : Migration
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
                name: "Auth",
                columns: table => new
                {
                    AuthKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortLivedKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongLivedKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth", x => x.AuthKey);
                    table.ForeignKey(
                        name: "FK_Auth_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vault",
                columns: table => new
                {
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaultCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaultType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vault", x => x.VaultId);
                    table.ForeignKey(
                        name: "FK_Vault_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auth_AccountId",
                table: "Auth",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Vault_AccountId",
                table: "Vault",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auth");

            migrationBuilder.DropTable(
                name: "Vault");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
