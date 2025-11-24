using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDataTypeOfInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TeamAcceptingInvites",
                table: "Team",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TeamAcceptingInvites",
                table: "Team",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
