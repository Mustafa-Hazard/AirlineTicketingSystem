using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Airport_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Iata_Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromAirport = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ToAirport = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EconomySeats = table.Column<int>(type: "int", nullable: false),
                    BusinessSeats = table.Column<int>(type: "int", nullable: false),
                    FirstClassSeats = table.Column<int>(type: "int", nullable: false),
                    AvailableEconomySeats = table.Column<int>(type: "int", nullable: false),
                    AvailableBusinessSeats = table.Column<int>(type: "int", nullable: false),
                    AvailableFirstClassSeats = table.Column<int>(type: "int", nullable: false),
                    EconomyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BusinessPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FirstClassPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDelayed = table.Column<bool>(type: "bit", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    First_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Last_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hashed_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    PassengerId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeatCount = table.Column<int>(type: "int", nullable: false),
                    SeatClass = table.Column<int>(type: "int", nullable: false),
                    PassengerType = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Id", "City", "Country", "Iata_Code", "Airport_Name" },
                values: new object[,]
                {
                    { 1, "Karachi", "Pakistan", "KHI", "Jinnah Intl" },
                    { 2, "Lahore", "Pakistan", "LHE", "Allama Iqbal Intl" },
                    { 3, "Islamabad", "Pakistan", "ISB", "Islamabad Intl" },
                    { 4, "Dubai", "UAE", "DXB", "Dubai Intl" },
                    { 5, "Abu Dhabi", "UAE", "AUH", "Zayed Intl" },
                    { 6, "Jeddah", "Saudi Arabia", "JED", "King Abdulaziz" },
                    { 7, "Riyadh", "Saudi Arabia", "RUH", "King Khalid" },
                    { 8, "London", "UK", "LHR", "Heathrow" },
                    { 9, "New York", "USA", "JFK", "JFK Intl" },
                    { 10, "Doha", "Qatar", "DOH", "Hamad Intl" },
                    { 11, "Bangkok", "Thailand", "BKK", "Suvarnabhumi" },
                    { 12, "Hong Kong", "China", "HKG", "Hong Kong Intl" },
                    { 13, "Multan", "Pakistan", "MUX", "Multan Intl" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FlightId",
                table: "Bookings",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PassengerId",
                table: "Bookings",
                column: "PassengerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Passengers");
        }
    }
}
