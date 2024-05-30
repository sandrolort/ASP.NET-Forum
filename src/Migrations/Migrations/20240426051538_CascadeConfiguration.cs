using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    public partial class CascadeConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6352F110-795A-4225-B923-1FF56DC3D255",
                column: "ConcurrencyStamp",
                value: "a62eb900-e128-4a43-8418-0b5bad89ecbf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "807FB3F0-EBA5-4169-8F98-6B9B217D493D",
                column: "ConcurrencyStamp",
                value: "cbacabbf-2471-45fd-85ce-4f693be24a43");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6352F110-795A-4225-B923-1FF56DC3D255",
                column: "ConcurrencyStamp",
                value: "03973cf1-a0a2-418d-80a5-4a3d544656f7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "807FB3F0-EBA5-4169-8F98-6B9B217D493D",
                column: "ConcurrencyStamp",
                value: "366f5187-52cb-41fd-820a-d6cd8cf2b186");
        }
    }
}
