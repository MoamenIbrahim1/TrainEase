using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    public partial class FixBookingUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Bookings_UserId1'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    DROP INDEX [IX_Bookings_UserId1] ON [Bookings];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Bookings', 'UserId1') IS NOT NULL
BEGIN
    ALTER TABLE [Bookings] DROP COLUMN [UserId1];
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Bookings', 'UserId1') IS NULL
BEGIN
    ALTER TABLE [Bookings] ADD [UserId1] nvarchar(450) NULL;
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Bookings_UserId1'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    CREATE INDEX [IX_Bookings_UserId1] ON [Bookings] ([UserId1]);
END
");
        }
    }
}