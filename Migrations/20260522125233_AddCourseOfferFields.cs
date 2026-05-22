using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseOfferFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeText",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Courses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BadgeColor", "BadgeText", "Description", "DiscountPrice", "ImageUrl" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BadgeColor", "BadgeText", "Description", "DiscountPrice", "ImageUrl" },
                values: new object[] { null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "BadgeText",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Courses");
        }
    }
}
