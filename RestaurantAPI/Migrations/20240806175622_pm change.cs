using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class pmchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts");

            migrationBuilder.AddColumn<int>(
                name: "Cid",
                table: "CheckInOuts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts",
                column: "Cid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts");

            migrationBuilder.DropColumn(
                name: "Cid",
                table: "CheckInOuts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts",
                column: "BookingId");
        }
    }
}
