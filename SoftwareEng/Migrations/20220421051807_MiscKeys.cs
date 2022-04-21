using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareEng.Migrations
{
    public partial class MiscKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChangedTo_OldReservationReservationID",
                table: "ChangedTo");

            migrationBuilder.DropIndex(
                name: "IX_BaseRatesReservations_BaseRatesBaseRateID",
                table: "BaseRatesReservations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChangedTo",
                table: "ChangedTo",
                columns: new[] { "OldReservationReservationID", "NewReservationReservationID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseRatesReservations",
                table: "BaseRatesReservations",
                columns: new[] { "BaseRatesBaseRateID", "ReservationsReservationID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChangedTo",
                table: "ChangedTo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseRatesReservations",
                table: "BaseRatesReservations");

            migrationBuilder.CreateIndex(
                name: "IX_ChangedTo_OldReservationReservationID",
                table: "ChangedTo",
                column: "OldReservationReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseRatesReservations_BaseRatesBaseRateID",
                table: "BaseRatesReservations",
                column: "BaseRatesBaseRateID");
        }
    }
}
