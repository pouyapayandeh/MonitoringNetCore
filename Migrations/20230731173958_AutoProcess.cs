using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    /// <inheritdoc />
    public partial class AutoProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoProcess",
                table: "Camera",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoProcess",
                table: "Camera");
        }
    }
}
