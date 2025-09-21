using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToReservationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Id",
                table: "Reservations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservation_Id",
                table: "Reservations");
        }
    }
}
