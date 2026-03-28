using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePassengerAndBookTicketBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Passengers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Passengers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Passengers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BookTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BookTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BookTickets",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BookTickets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BookTickets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BookTickets");
        }
    }
}
