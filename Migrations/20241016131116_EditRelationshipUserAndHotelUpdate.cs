using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trainning.Migrations
{
    /// <inheritdoc />
    public partial class EditRelationshipUserAndHotelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HotelId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HotelId1",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HotelId",
                table: "Users",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HotelId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "HotelId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
