using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    /// <inheritdoc />
    public partial class activeprocess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActiveProcess",
                table: "Camera",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActiveProcess",
                table: "Camera");
        }
    }
}
