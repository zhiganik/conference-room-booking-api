using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceRoomBooking.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNameSnapshotsToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceOptionName",
                table: "BookingServiceOptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Bookings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Backfill existing rows from the current Rooms/ServiceOptions data (including
            // soft-deleted rooms, since this raw SQL bypasses the EF global query filter) —
            // new rows going forward are populated by the application at booking time.
            migrationBuilder.Sql("""
                UPDATE b
                SET b.RoomName = r.Name
                FROM Bookings b
                INNER JOIN Rooms r ON r.Id = b.RoomId;
                """);

            migrationBuilder.Sql("""
                UPDATE bso
                SET bso.ServiceOptionName = s.Name
                FROM BookingServiceOptions bso
                INNER JOIN ServiceOptions s ON s.Id = bso.ServiceOptionId;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceOptionName",
                table: "BookingServiceOptions");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Bookings");
        }
    }
}
