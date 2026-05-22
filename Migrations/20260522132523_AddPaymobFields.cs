using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymobFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymobOrderId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymobOrderId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Enrollments");
        }
    }
}
