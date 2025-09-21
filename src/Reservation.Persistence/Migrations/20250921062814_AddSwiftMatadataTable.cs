using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reservation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSwiftMatadataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwiftMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BasicHeader = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ApplicationHeader = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UserHeader = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TextBlock = table.Column<string>(type: "text", nullable: true),
                    Trailer = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsMt103Plus = table.Column<bool>(type: "boolean", nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ValueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Beneficiary = table.Column<string>(type: "text", nullable: true),
                    OrderingCustomer = table.Column<string>(type: "text", nullable: true),
                    Mac = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Checksum = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwiftMetadata", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwiftMetadata");
        }
    }
}
