using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePassport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Passengers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Id", "City", "Country", "Iata_Code", "Airport_Name" },
                values: new object[,]
                {
                    { 11, "Faisalabad", "Pakistan", "LYP", "Faisalabad Intl" },
                    { 12, "Multan", "Pakistan", "MUX", "Multan Intl" },
                    { 13, "Quetta", "Pakistan", "UET", "Quetta Intl" },
                    { 14, "Sialkot", "Pakistan", "SKT", "Sialkot Intl" },
                    { 15, "Skardu", "Pakistan", "KDU", "Skardu Airport" },
                    { 16, "Mumbai", "India", "BOM", "Chhatrapati Shivaji" },
                    { 17, "Delhi", "India", "DEL", "Indira Gandhi Intl" },
                    { 18, "Bangalore", "India", "BLR", "Kempegowda Intl" },
                    { 19, "Chennai", "India", "MAA", "Chennai Intl" },
                    { 20, "Hyderabad", "India", "HYD", "Rajiv Gandhi Intl" },
                    { 21, "Singapore", "Singapore", "SIN", "Changi Airport" },
                    { 22, "Tokyo", "Japan", "HND", "Haneda Airport" },
                    { 23, "Tokyo", "Japan", "NRT", "Narita Intl" },
                    { 24, "Beijing", "China", "PEK", "Capital Intl" },
                    { 25, "Shanghai", "China", "PVG", "Pudong Intl" },
                    { 26, "Hong Kong", "China", "HKG", "Hong Kong Intl" },
                    { 27, "Seoul", "South Korea", "ICN", "Incheon Intl" },
                    { 28, "Bangkok", "Thailand", "BKK", "Suvarnabhumi Airport" },
                    { 29, "Kuala Lumpur", "Malaysia", "KUL", "Kuala Lumpur Intl" },
                    { 30, "Istanbul", "Turkey", "IST", "Istanbul Airport" },
                    { 31, "Paris", "France", "CDG", "Charles de Gaulle" },
                    { 32, "Frankfurt", "Germany", "FRA", "Frankfurt Airport" },
                    { 33, "Munich", "Germany", "MUC", "Munich Airport" },
                    { 34, "Amsterdam", "Netherlands", "AMS", "Schiphol Airport" },
                    { 35, "Madrid", "Spain", "MAD", "Adolfo Suárez" },
                    { 36, "Barcelona", "Spain", "BCN", "El Prat Airport" },
                    { 37, "Rome", "Italy", "FCO", "Fiumicino Airport" },
                    { 38, "Zurich", "Switzerland", "ZRH", "Zurich Airport" },
                    { 39, "Moscow", "Russia", "SVO", "Sheremetyevo Intl" },
                    { 40, "London", "UK", "LGW", "Gatwick Airport" },
                    { 41, "Manchester", "UK", "MAN", "Manchester Airport" },
                    { 42, "Birmingham", "UK", "BHX", "Birmingham Airport" },
                    { 43, "Los Angeles", "USA", "LAX", "LAX Intl" },
                    { 44, "Chicago", "USA", "ORD", "O'Hare Intl" },
                    { 45, "Dallas", "USA", "DFW", "Dallas/Fort Worth" },
                    { 46, "Atlanta", "USA", "ATL", "Hartsfield-Jackson" },
                    { 47, "San Francisco", "USA", "SFO", "SFO Intl" },
                    { 48, "Miami", "USA", "MIA", "Miami Intl" },
                    { 49, "Toronto", "Canada", "YYZ", "Pearson Intl" },
                    { 50, "Vancouver", "Canada", "YVR", "Vancouver Intl" },
                    { 51, "Sydney", "Australia", "SYD", "Kingsford Smith" },
                    { 52, "Melbourne", "Australia", "MEL", "Melbourne Airport" },
                    { 53, "Auckland", "New Zealand", "AKL", "Auckland Airport" },
                    { 54, "Johannesburg", "South Africa", "JNB", "OR Tambo Intl" },
                    { 55, "Cairo", "Egypt", "CAI", "Cairo Intl" },
                    { 56, "Muscat", "Oman", "MCT", "Muscat Intl" },
                    { 57, "Kuwait City", "Kuwait", "KWI", "Kuwait Intl" },
                    { 58, "Manama", "Bahrain", "BAH", "Bahrain Intl" },
                    { 59, "Tehran", "Iran", "IKA", "Imam Khomeini Intl" },
                    { 60, "Colombo", "Sri Lanka", "CMB", "Bandaranaike Intl" },
                    { 61, "Dhaka", "Bangladesh", "DAC", "Hazrat Shahjalal" },
                    { 62, "Kathmandu", "Nepal", "KTM", "Tribhuvan Intl" },
                    { 63, "Male", "Maldives", "MLE", "Velana Intl" },
                    { 64, "Jakarta", "Indonesia", "CGK", "Soekarno-Hatta" },
                    { 65, "Manila", "Philippines", "MNL", "Ninoy Aquino Intl" },
                    { 66, "Ho Chi Minh", "Vietnam", "SGN", "Tan Son Nhat" },
                    { 67, "Taipei", "Taiwan", "TPE", "Taoyuan Intl" },
                    { 68, "Lisbon", "Portugal", "LIS", "Humberto Delgado" },
                    { 69, "Stockholm", "Sweden", "ARN", "Arlanda Airport" },
                    { 70, "Oslo", "Norway", "OSL", "Oslo Airport" },
                    { 71, "Copenhagen", "Denmark", "CPH", "Copenhagen Airport" },
                    { 72, "Helsinki", "Finland", "HEL", "Helsinki Airport" },
                    { 73, "Vienna", "Austria", "VIE", "Vienna Intl" },
                    { 74, "Brussels", "Belgium", "BRU", "Brussels Airport" },
                    { 75, "Athens", "Greece", "ATH", "Athens Intl" },
                    { 76, "Prague", "Czech Republic", "PRG", "Václav Havel" },
                    { 77, "Warsaw", "Poland", "WAW", "Chopin Airport" },
                    { 78, "Dublin", "Ireland", "DUB", "Dublin Airport" },
                    { 79, "Edinburgh", "UK", "EDI", "Edinburgh Airport" },
                    { 80, "Boston", "USA", "BOS", "Logan Intl" },
                    { 81, "Washington", "USA", "IAD", "Dulles Intl" },
                    { 82, "Houston", "USA", "IAH", "George Bush Intl" },
                    { 83, "Seattle", "USA", "SEA", "Seattle-Tacoma" },
                    { 84, "Denver", "USA", "DEN", "Denver Intl" },
                    { 85, "Las Vegas", "USA", "LAS", "Harry Reid Intl" },
                    { 86, "Mexico City", "Mexico", "MEX", "Benito Juárez" },
                    { 87, "Sao Paulo", "Brazil", "GRU", "Guarulhos Intl" },
                    { 88, "Buenos Aires", "Argentina", "EZE", "Ministro Pistarini" },
                    { 89, "Santiago", "Chile", "SCL", "Arturo Merino" },
                    { 90, "Lima", "Peru", "LIM", "Jorge Chávez" },
                    { 91, "Bogota", "Colombia", "BOG", "El Dorado Intl" },
                    { 92, "Nairobi", "Kenya", "NBO", "Jomo Kenyatta" },
                    { 93, "Casablanca", "Morocco", "CMN", "Mohammed V" },
                    { 94, "Addis Ababa", "Ethiopia", "ADD", "Bole Intl" },
                    { 95, "Cape Town", "South Africa", "CPT", "Cape Town Intl" },
                    { 96, "Tel Aviv", "Israel", "TLV", "Ben Gurion" },
                    { 97, "Amman", "Jordan", "AMM", "Queen Alia Intl" },
                    { 98, "Beirut", "Lebanon", "BEY", "Rafic Hariri" },
                    { 99, "Baghdad", "Iraq", "BGW", "Baghdad Intl" },
                    { 100, "Perth", "Australia", "PER", "Perth Airport" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassportNumber",
                table: "Passengers",
                column: "PassportNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassportNumber_UserId",
                table: "Passengers",
                columns: new[] { "PassportNumber", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_PassportNumber",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_PassportNumber_UserId",
                table: "Passengers");

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
