using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class addingid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInChecks",
                table: "CheckInChecks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CheckInChecks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CheckInChecks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckInChecks",
                table: "CheckInChecks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInChecks",
                table: "CheckInChecks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CheckInChecks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CheckInChecks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckInChecks",
                table: "CheckInChecks",
                column: "UserId");
        }
    }
}
