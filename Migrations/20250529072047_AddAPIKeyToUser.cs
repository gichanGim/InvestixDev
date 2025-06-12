using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestixDev.Migrations
{
    /// <inheritdoc />
    public partial class AddAPIKeyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppSecret",
                table: "AspNetUsers");
        }
    }
}
