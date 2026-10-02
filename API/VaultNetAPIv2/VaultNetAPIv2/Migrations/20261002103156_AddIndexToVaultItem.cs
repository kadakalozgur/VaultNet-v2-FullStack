using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultNetAPIv2.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToVaultItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_UserId_CreatedAt",
                table: "VaultItems",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaultItems_UserId_CreatedAt",
                table: "VaultItems");
        }
    }
}
