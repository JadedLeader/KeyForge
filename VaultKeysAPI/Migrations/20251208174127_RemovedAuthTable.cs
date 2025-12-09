using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultKeysAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAuthTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auth",
                columns: table => new
                {
                    AuthKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LongLivedKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
