using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareEng.Migrations
{
    public partial class RemoveExplicitJoinEntityClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseRatesReservations1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseRatesReservations1",
                columns: table => new
                {
                    BaseRatesBaseRateID = table.Column<int>(type: "int", nullable: false),
                    ReservationsReservationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRatesReservations1", x => new { x.BaseRatesBaseRateID, x.ReservationsReservationID });
                    table.ForeignKey(
                        name: "FK_BaseRatesReservations1_BaseRates_BaseRatesBaseRateID",
                        column: x => x.BaseRatesBaseRateID,
                        principalTable: "BaseRates",
                        principalColumn: "BaseRateID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseRatesReservations1_Reservations_ReservationsReservationID",
                        column: x => x.ReservationsReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseRatesReservations1_ReservationsReservationID",
                table: "BaseRatesReservations1",
                column: "ReservationsReservationID");
        }
    }
}
