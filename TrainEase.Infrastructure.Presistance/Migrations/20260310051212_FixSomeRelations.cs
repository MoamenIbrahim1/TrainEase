using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class FixSomeRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Schedule",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "RouteStation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "Route",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "RouteStation");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "Route");
        }
    }
}
