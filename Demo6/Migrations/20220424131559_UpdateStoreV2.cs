using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo6.Migrations
{
    public partial class UpdateStoreV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Carts_UId",
                table: "Carts",
                column: "UId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_UId",
                table: "Carts");
        }
    }
}
