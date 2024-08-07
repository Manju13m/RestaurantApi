using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts");

            migrationBuilder.DropColumn(
                name: "Cid",
                table: "CheckInOuts");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "CheckInOuts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckInOuts",
                table: "CheckInOuts");

            migrationBuilder.DropColumn(
                name: "id",
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
    }
}
