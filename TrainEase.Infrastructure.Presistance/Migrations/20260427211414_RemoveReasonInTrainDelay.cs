using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReasonInTrainDelay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "TrainDelays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "TrainDelays",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
