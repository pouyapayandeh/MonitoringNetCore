using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    public partial class AddUrlColumnToCamera : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                    name: "Url",
                    table: "Camera",
                    type: "TEXT",
                    nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
