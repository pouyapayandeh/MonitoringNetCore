using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    /// <inheritdoc />
    public partial class VideoProcessJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoFile_OriginalVideoId",
                table: "VideoFile");

            migrationBuilder.CreateIndex(
                name: "IX_VideoFile_OriginalVideoId",
                table: "VideoFile",
                column: "OriginalVideoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoFile_OriginalVideoId",
                table: "VideoFile");

            migrationBuilder.CreateIndex(
                name: "IX_VideoFile_OriginalVideoId",
                table: "VideoFile",
                column: "OriginalVideoId");
        }
    }
}
