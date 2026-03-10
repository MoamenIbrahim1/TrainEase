using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainEase.Infrastructure.Presistance.Migrations
{
    /// <inheritdoc />
    public partial class AddEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_AspNetUsers_UserId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Carriage_Train_TrainId",
                table: "Carriage");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Booking_BookingId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_SentByUserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Booking_BookingId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Booking_BookingId",
                table: "Refund");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStation_Route_RouteId",
                table: "RouteStation");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStation_Station_StationId",
                table: "RouteStation");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Route_RouteId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Station_DestinationStationId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Station_SourceStationId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Train_TrainId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Seat_Carriage_CarriageId",
                table: "Seat");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AspNetUsers_UserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Booking_BookingId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Seat_SeatId",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Train",
                table: "Train");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Station",
                table: "Station");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seat",
                table: "Seat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RouteStation",
                table: "RouteStation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Route",
                table: "Route");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refund",
                table: "Refund");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carriage",
                table: "Carriage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "Train",
                newName: "Trains");

            migrationBuilder.RenameTable(
                name: "Ticket",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "Station",
                newName: "Stations");

            migrationBuilder.RenameTable(
                name: "Seat",
                newName: "Seats");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "Schedules");

            migrationBuilder.RenameTable(
                name: "RouteStation",
                newName: "RouteStations");

            migrationBuilder.RenameTable(
                name: "Route",
                newName: "Routes");

            migrationBuilder.RenameTable(
                name: "Refund",
                newName: "Refunds");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "Carriage",
                newName: "Carriages");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_UserId",
                table: "Tickets",
                newName: "IX_Tickets_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_SeatId",
                table: "Tickets",
                newName: "IX_Tickets_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_Ticket_BookingId",
                table: "Tickets",
                newName: "IX_Tickets_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Seat_CarriageId",
                table: "Seats",
                newName: "IX_Seats_CarriageId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_TrainId",
                table: "Schedules",
                newName: "IX_Schedules_TrainId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_SourceStationId",
                table: "Schedules",
                newName: "IX_Schedules_SourceStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_RouteId",
                table: "Schedules",
                newName: "IX_Schedules_RouteId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_DestinationStationId",
                table: "Schedules",
                newName: "IX_Schedules_DestinationStationId");

            migrationBuilder.RenameIndex(
                name: "IX_RouteStation_StationId",
                table: "RouteStations",
                newName: "IX_RouteStations_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Refund_BookingId",
                table: "Refunds",
                newName: "IX_Refunds_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_BookingId",
                table: "Payments",
                newName: "IX_Payments_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SentByUserId",
                table: "Notifications",
                newName: "IX_Notifications_SentByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_BookingId",
                table: "Invoices",
                newName: "IX_Invoices_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Carriage_TrainId",
                table: "Carriages",
                newName: "IX_Carriages_TrainId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_UserId",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trains",
                table: "Trains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stations",
                table: "Stations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seats",
                table: "Seats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RouteStations",
                table: "RouteStations",
                columns: new[] { "RouteId", "StationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routes",
                table: "Routes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carriages",
                table: "Carriages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carriages_Trains_TrainId",
                table: "Carriages",
                column: "TrainId",
                principalTable: "Trains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SentByUserId",
                table: "Notifications",
                column: "SentByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Bookings_BookingId",
                table: "Refunds",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStations_Routes_RouteId",
                table: "RouteStations",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStations_Stations_StationId",
                table: "RouteStations",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Routes_RouteId",
                table: "Schedules",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Trains_TrainId",
                table: "Schedules",
                column: "TrainId",
                principalTable: "Trains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Carriages_CarriageId",
                table: "Seats",
                column: "CarriageId",
                principalTable: "Carriages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_UserId",
                table: "Tickets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Bookings_BookingId",
                table: "Tickets",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Seats_SeatId",
                table: "Tickets",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Carriages_Trains_TrainId",
                table: "Carriages");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Bookings_BookingId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SentByUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Bookings_BookingId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStations_Routes_RouteId",
                table: "RouteStations");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStations_Stations_StationId",
                table: "RouteStations");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Routes_RouteId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_DestinationStationId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Stations_SourceStationId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Trains_TrainId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Carriages_CarriageId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_UserId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Bookings_BookingId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Seats_SeatId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trains",
                table: "Trains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tickets",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stations",
                table: "Stations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seats",
                table: "Seats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RouteStations",
                table: "RouteStations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Routes",
                table: "Routes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carriages",
                table: "Carriages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Trains",
                newName: "Train");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Ticket");

            migrationBuilder.RenameTable(
                name: "Stations",
                newName: "Station");

            migrationBuilder.RenameTable(
                name: "Seats",
                newName: "Seat");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameTable(
                name: "RouteStations",
                newName: "RouteStation");

            migrationBuilder.RenameTable(
                name: "Routes",
                newName: "Route");

            migrationBuilder.RenameTable(
                name: "Refunds",
                newName: "Refund");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "Carriages",
                newName: "Carriage");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_UserId",
                table: "Ticket",
                newName: "IX_Ticket_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_SeatId",
                table: "Ticket",
                newName: "IX_Ticket_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_BookingId",
                table: "Ticket",
                newName: "IX_Ticket_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_CarriageId",
                table: "Seat",
                newName: "IX_Seat_CarriageId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_TrainId",
                table: "Schedule",
                newName: "IX_Schedule_TrainId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_SourceStationId",
                table: "Schedule",
                newName: "IX_Schedule_SourceStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_RouteId",
                table: "Schedule",
                newName: "IX_Schedule_RouteId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_DestinationStationId",
                table: "Schedule",
                newName: "IX_Schedule_DestinationStationId");

            migrationBuilder.RenameIndex(
                name: "IX_RouteStations_StationId",
                table: "RouteStation",
                newName: "IX_RouteStation_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_BookingId",
                table: "Refund",
                newName: "IX_Refund_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_BookingId",
                table: "Payment",
                newName: "IX_Payment_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SentByUserId",
                table: "Notification",
                newName: "IX_Notification_SentByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoice",
                newName: "IX_Invoice_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Carriages_TrainId",
                table: "Carriage",
                newName: "IX_Carriage_TrainId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "Booking",
                newName: "IX_Booking_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Train",
                table: "Train",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ticket",
                table: "Ticket",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Station",
                table: "Station",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seat",
                table: "Seat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RouteStation",
                table: "RouteStation",
                columns: new[] { "RouteId", "StationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Route",
                table: "Route",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refund",
                table: "Refund",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carriage",
                table: "Carriage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_AspNetUsers_UserId",
                table: "Booking",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carriage_Train_TrainId",
                table: "Carriage",
                column: "TrainId",
                principalTable: "Train",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Booking_BookingId",
                table: "Invoice",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_SentByUserId",
                table: "Notification",
                column: "SentByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Booking_BookingId",
                table: "Payment",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Booking_BookingId",
                table: "Refund",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStation_Route_RouteId",
                table: "RouteStation",
                column: "RouteId",
                principalTable: "Route",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStation_Station_StationId",
                table: "RouteStation",
                column: "StationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Route_RouteId",
                table: "Schedule",
                column: "RouteId",
                principalTable: "Route",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Station_DestinationStationId",
                table: "Schedule",
                column: "DestinationStationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Station_SourceStationId",
                table: "Schedule",
                column: "SourceStationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Train_TrainId",
                table: "Schedule",
                column: "TrainId",
                principalTable: "Train",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seat_Carriage_CarriageId",
                table: "Seat",
                column: "CarriageId",
                principalTable: "Carriage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AspNetUsers_UserId",
                table: "Ticket",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Booking_BookingId",
                table: "Ticket",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Seat_SeatId",
                table: "Ticket",
                column: "SeatId",
                principalTable: "Seat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
