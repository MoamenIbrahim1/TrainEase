using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    DepartedFromStationId = table.Column<int>(type: "int", nullable: false),
                    HeadingToStationId = table.Column<int>(type: "int", nullable: false),
                    ActualDepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDistanceMeters = table.Column<double>(type: "float", nullable: false),
                    DelayMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainTrackings_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainTrackings_Stations_DepartedFromStationId",
                        column: x => x.DepartedFromStationId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainTrackings_Stations_HeadingToStationId",
                        column: x => x.HeadingToStationId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainTrackings_DepartedFromStationId",
                table: "TrainTrackings",
                column: "DepartedFromStationId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainTrackings_HeadingToStationId",
                table: "TrainTrackings",
                column: "HeadingToStationId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainTrackings_ScheduleId",
                table: "TrainTrackings",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainTrackings");
        }
    }
}
