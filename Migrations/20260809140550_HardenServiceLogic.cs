using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicAppoinment.Migrations
{
    /// <inheritdoc />
    public partial class HardenServiceLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AvailabilitySlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments",
                column: "AvailabilitySlotId",
                unique: true,
                filter: "[Status] IN ('Pending', 'Confirmed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AvailabilitySlots");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments",
                column: "AvailabilitySlotId",
                unique: true);
        }
    }
}
