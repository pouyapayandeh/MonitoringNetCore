using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isProcessed",
                table: "VideoFile",
                newName: "IsProcessed");

            migrationBuilder.RenameColumn(
                name: "isActiveProcess",
                table: "Camera",
                newName: "IsRealtimeProcess");

            migrationBuilder.RenameColumn(
                name: "AutoProcess",
                table: "Camera",
                newName: "AutoSave");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProcessed",
                table: "VideoFile",
                newName: "isProcessed");

            migrationBuilder.RenameColumn(
                name: "IsRealtimeProcess",
                table: "Camera",
                newName: "isActiveProcess");

            migrationBuilder.RenameColumn(
                name: "AutoSave",
                table: "Camera",
                newName: "AutoProcess");
        }
    }
}
