using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserRoleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019931ab-a038-78cd-b48c-a587b7f3d33f", "019931ab-a038-78cd-b48c-a58882b3f8fa", false, false, "Admin", "ADMIN" },
                    { "019931ab-a038-78cd-b48c-a589117d2362", "019931ab-a038-78cd-b48c-a58a7e3e5887", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019931ab-a038-78cd-b48c-a58420e83135", 0, "019931ab-a038-78cd-b48c-a5860750e1d9", "Admin@SurvayBasket.com", true, "Ibrahim", false, "khaled", false, null, "ADMIN@SURVAYBASKET.COM", "IIBRAHIM", "AQAAAAIAAYagAAAAECFdYRH0N9Q/1vWopOwqjWken0x7biV7Sa7slwy+LrjeWIn3UFAmPhkTk3N+HtX1AQ==", null, false, "019931aba03878cdb48ca585cd54f0d1", false, "iibrahim" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019931ab-a038-78cd-b48c-a587b7f3d33f", "019931ab-a038-78cd-b48c-a58420e83135" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019931ab-a038-78cd-b48c-a589117d2362");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019931ab-a038-78cd-b48c-a587b7f3d33f", "019931ab-a038-78cd-b48c-a58420e83135" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019931ab-a038-78cd-b48c-a587b7f3d33f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019931ab-a038-78cd-b48c-a58420e83135");
        }
    }
}
