using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedAirport : Migration
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
                    { 10, "Doha", "Qatar", "DOH", "Hamad Intl" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
