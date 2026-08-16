using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceRoomBooking.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingServicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Rooms_RoomId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServiceOption_Booking_BookingId",
                table: "BookingServiceOption");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServiceOption_ServiceOptions_ServiceOptionId",
                table: "BookingServiceOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingServiceOption",
                table: "BookingServiceOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "BookingServiceOption",
                newName: "BookingServiceOptions");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_BookingServiceOption_ServiceOptionId",
                table: "BookingServiceOptions",
                newName: "IX_BookingServiceOptions_ServiceOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_RoomId",
                table: "Bookings",
                newName: "IX_Bookings_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingServiceOptions",
                table: "BookingServiceOptions",
                columns: new[] { "BookingId", "ServiceOptionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServiceOptions_Bookings_BookingId",
                table: "BookingServiceOptions",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServiceOptions_ServiceOptions_ServiceOptionId",
                table: "BookingServiceOptions",
                column: "ServiceOptionId",
                principalTable: "ServiceOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServiceOptions_Bookings_BookingId",
                table: "BookingServiceOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServiceOptions_ServiceOptions_ServiceOptionId",
                table: "BookingServiceOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingServiceOptions",
                table: "BookingServiceOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "BookingServiceOptions",
                newName: "BookingServiceOption");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_BookingServiceOptions_ServiceOptionId",
                table: "BookingServiceOption",
                newName: "IX_BookingServiceOption_ServiceOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_RoomId",
                table: "Booking",
                newName: "IX_Booking_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingServiceOption",
                table: "BookingServiceOption",
                columns: new[] { "BookingId", "ServiceOptionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Rooms_RoomId",
                table: "Booking",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServiceOption_Booking_BookingId",
                table: "BookingServiceOption",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServiceOption_ServiceOptions_ServiceOptionId",
                table: "BookingServiceOption",
                column: "ServiceOptionId",
                principalTable: "ServiceOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
