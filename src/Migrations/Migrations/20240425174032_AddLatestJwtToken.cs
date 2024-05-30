using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    public partial class AddLatestJwtToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastJwtToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastJwtToken",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6352F110-795A-4225-B923-1FF56DC3D255",
                column: "ConcurrencyStamp",
                value: "f85f5cab-cd07-4566-892f-0a99d87eb9f8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "807FB3F0-EBA5-4169-8F98-6B9B217D493D",
                column: "ConcurrencyStamp",
                value: "d8c215e0-8e76-4149-9df8-4d004f708918");
        }
    }
}
