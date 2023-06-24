using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    public partial class ThumbnailToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "VideoFile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "VideoFile");
        }
    }
}
