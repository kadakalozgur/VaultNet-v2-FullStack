using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultNetAPIv2.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIndexToVaultItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaultItems_UserId",
                table: "VaultItems");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_UserId_CategoryId",
                table: "VaultItems",
                columns: new[] { "UserId", "CategoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaultItems_UserId_CategoryId",
                table: "VaultItems");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_UserId",
                table: "VaultItems",
                column: "UserId");
        }
    }
}
