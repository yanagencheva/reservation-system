using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessfulAndFailedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DT = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Raw_Request = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessfulReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DT = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Raw_Request = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessfulReservations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedReservations");

            migrationBuilder.DropTable(
                name: "SuccessfulReservations");
        }
    }
}
