using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAuthTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auth");

            migrationBuilder.CreateTable(
                name: "VaultKeysDataModel",
                columns: table => new
                {
                    VaultKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashedVaultKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimeVaultKeyCreated = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultKeysDataModel", x => x.VaultKeyId);
                    table.ForeignKey(
                        name: "FK_VaultKeysDataModel_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "VaultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaultKeysDataModel_VaultId",
                table: "VaultKeysDataModel",
                column: "VaultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaultKeysDataModel");

            migrationBuilder.CreateTable(
                name: "Auth",
                columns: table => new
                {
                    AuthKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LongLivedKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortLivedKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Auth_AccountId",
                table: "Auth",
                column: "AccountId");
        }
    }
}
