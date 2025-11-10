using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultKeysAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingKeyNameAndTimeCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateTimeVaultKeyCreated",
                table: "VaultKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeyName",
                table: "VaultKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTimeVaultKeyCreated",
                table: "VaultKeys");

            migrationBuilder.DropColumn(
                name: "KeyName",
                table: "VaultKeys");
        }
    }
}
