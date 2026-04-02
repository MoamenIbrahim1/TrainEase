using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_DestinationStationId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_SourceStationId",
                table: "Schedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Stations_DestinationStationId",
                table: "Schedules",
                column: "DestinationStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Stations_SourceStationId",
                table: "Schedules",
                column: "SourceStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_DestinationStationId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_SourceStationId",
                table: "Schedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Stations_DestinationStationId",
                table: "Schedules",
                column: "DestinationStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Stations_SourceStationId",
                table: "Schedules",
                column: "SourceStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
