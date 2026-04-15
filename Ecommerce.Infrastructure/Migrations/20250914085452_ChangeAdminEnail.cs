using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminEnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019931ab-a038-78cd-b48c-a58420e83135",
                columns: new[] { "Email", "NormalizedEmail" },
                values: new object[] { "Admin@Ecommerce.com", "ADMIN@ECOMMERCE.COM" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019931ab-a038-78cd-b48c-a58420e83135",
                columns: new[] { "Email", "NormalizedEmail" },
                values: new object[] { "Admin@SurvayBasket.com", "ADMIN@SURVAYBASKET.COM" });
        }
    }
}
