using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    public partial class RefactorBookingPassengerTicketRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Bookings_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Bookings] DROP CONSTRAINT [FK_Bookings_Passengers_PassengerId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Bookings_PassengerId'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    DROP INDEX [IX_Bookings_PassengerId] ON [Bookings];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Bookings', 'PassengerId') IS NOT NULL
BEGIN
    ALTER TABLE [Bookings] DROP COLUMN [PassengerId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Tickets_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Tickets] DROP CONSTRAINT [FK_Tickets_Passengers_PassengerId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tickets_PassengerId'
      AND object_id = OBJECT_ID(N'[Tickets]')
)
BEGIN
    DROP INDEX [IX_Tickets_PassengerId] ON [Tickets];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[BookTickets]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [BookTickets];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Tickets', 'PassengerId') IS NULL
BEGIN
    ALTER TABLE [Tickets] ADD [PassengerId] int NULL;
END
");

            migrationBuilder.Sql(@"
UPDATE [Tickets]
SET [PassengerId] = 1
WHERE [PassengerId] IS NULL
");

            migrationBuilder.Sql(@"
DECLARE @DefaultConstraintName sysname;

SELECT @DefaultConstraintName = d.name
FROM sys.default_constraints d
INNER JOIN sys.columns c
    ON d.parent_object_id = c.object_id
   AND d.parent_column_id = c.column_id
WHERE d.parent_object_id = OBJECT_ID(N'[Tickets]')
  AND c.name = N'PassengerId';

IF @DefaultConstraintName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE [Tickets] DROP CONSTRAINT [' + @DefaultConstraintName + ']');
END
");

            migrationBuilder.Sql(@"
ALTER TABLE [Tickets] ALTER COLUMN [PassengerId] int NOT NULL;
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tickets_PassengerId'
      AND object_id = OBJECT_ID(N'[Tickets]')
)
BEGIN
    CREATE INDEX [IX_Tickets_PassengerId] ON [Tickets]([PassengerId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Tickets_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Tickets]
    ADD CONSTRAINT [FK_Tickets_Passengers_PassengerId]
    FOREIGN KEY ([PassengerId]) REFERENCES [Passengers]([Id]) ON DELETE NO ACTION;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Tickets_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Tickets] DROP CONSTRAINT [FK_Tickets_Passengers_PassengerId];
END
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tickets_PassengerId'
      AND object_id = OBJECT_ID(N'[Tickets]')
)
BEGIN
    DROP INDEX [IX_Tickets_PassengerId] ON [Tickets];
END
");

            migrationBuilder.Sql(@"
DECLARE @DefaultConstraintName sysname;

SELECT @DefaultConstraintName = d.name
FROM sys.default_constraints d
INNER JOIN sys.columns c
    ON d.parent_object_id = c.object_id
   AND d.parent_column_id = c.column_id
WHERE d.parent_object_id = OBJECT_ID(N'[Tickets]')
  AND c.name = N'PassengerId';

IF @DefaultConstraintName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE [Tickets] DROP CONSTRAINT [' + @DefaultConstraintName + ']');
END
");

            migrationBuilder.Sql(@"
ALTER TABLE [Tickets] ALTER COLUMN [PassengerId] int NULL;
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Bookings', 'PassengerId') IS NULL
BEGIN
    ALTER TABLE [Bookings] ADD [PassengerId] int NULL;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[BookTickets]', N'U') IS NULL
BEGIN
    CREATE TABLE [BookTickets](
        [Id] int NOT NULL IDENTITY(1,1),
        [PassengerId] int NOT NULL,
        [TicketId] int NOT NULL,
        [CreatedAt] datetime2 NULL,
        [CreatedOn] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_BookTickets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookTickets_Passengers_PassengerId] FOREIGN KEY ([PassengerId]) REFERENCES [Passengers]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookTickets_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id]) ON DELETE CASCADE
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Bookings_PassengerId'
      AND object_id = OBJECT_ID(N'[Bookings]')
)
BEGIN
    CREATE INDEX [IX_Bookings_PassengerId] ON [Bookings]([PassengerId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BookTickets_PassengerId'
      AND object_id = OBJECT_ID(N'[BookTickets]')
)
BEGIN
    CREATE INDEX [IX_BookTickets_PassengerId] ON [BookTickets]([PassengerId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BookTickets_TicketId'
      AND object_id = OBJECT_ID(N'[BookTickets]')
)
BEGIN
    CREATE UNIQUE INDEX [IX_BookTickets_TicketId] ON [BookTickets]([TicketId]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Bookings_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Bookings]
    ADD CONSTRAINT [FK_Bookings_Passengers_PassengerId]
    FOREIGN KEY ([PassengerId]) REFERENCES [Passengers]([Id]);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Tickets_Passengers_PassengerId'
)
BEGIN
    ALTER TABLE [Tickets]
    ADD CONSTRAINT [FK_Tickets_Passengers_PassengerId]
    FOREIGN KEY ([PassengerId]) REFERENCES [Passengers]([Id]);
END
");
        }
    }
}