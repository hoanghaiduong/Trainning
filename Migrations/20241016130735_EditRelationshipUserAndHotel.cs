using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainning.Migrations
{
    /// <inheritdoc />
    public partial class EditRelationshipUserAndHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HotelId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelId1",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HotelId1",
                table: "Users",
                column: "HotelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelId1",
                table: "Users",
                column: "HotelId1",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HotelId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HotelId1",
                table: "Users");
        }
    }
}
