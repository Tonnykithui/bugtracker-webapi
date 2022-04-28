using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketSystemAPI.Migrations
{
    public partial class EstimateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstimateTime",
                table: "Tickets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimateTime",
                table: "Tickets");
        }
    }
}
