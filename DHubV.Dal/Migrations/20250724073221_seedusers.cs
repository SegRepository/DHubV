using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHubV.Dal.Migrations
{
    /// <inheritdoc />
    public partial class seedusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "512f6df4-e4b2-4b1f-a01e-60ace1761cb3", 0, "ee1039b3-ee74-41a9-8b73-8b5f74e4a7fb", "Mohsen@gmail.com", true, "محمد محسن", false, null, "MOHSEN", "MOHSEN@GMAIL.COM", "AQAAAAIAAYagAAAAEHe61zEw3gk2CVMdtPl5FAbQPzwxIYWhe96GqN3RywiDuojearPtXbaN0j6TC2mwIA==", null, false, "WGKUB4EBB2DV7CYBY65WXVWRSODM3D6U", false, "Mohsen" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "512f6df4-e4b2-4b1f-a01e-60ace1761cb3");
        }
    }
}
