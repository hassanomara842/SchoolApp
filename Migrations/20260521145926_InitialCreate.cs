using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manager = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<int>(type: "int", nullable: false),
                    MinDegree = table.Column<int>(type: "int", nullable: false),
                    Hrs = table.Column<int>(type: "int", nullable: false),
                    Dept_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_Dept_id",
                        column: x => x.Dept_id,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trainees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dept_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trainees_Departments_Dept_id",
                        column: x => x.Dept_id,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dept_id = table.Column<int>(type: "int", nullable: false),
                    Crs_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructors_Courses_Crs_id",
                        column: x => x.Crs_id,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Instructors_Departments_Dept_id",
                        column: x => x.Dept_id,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CrsResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Degree = table.Column<int>(type: "int", nullable: false),
                    Crs_id = table.Column<int>(type: "int", nullable: false),
                    Trainee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrsResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrsResults_Courses_Crs_id",
                        column: x => x.Crs_id,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrsResults_Trainees_Trainee_id",
                        column: x => x.Trainee_id,
                        principalTable: "Trainees",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Manager", "Name" },
                values: new object[,]
                {
                    { 1, "Ahmed", "Computer Science" },
                    { 2, "Sara", "Information Systems" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Degree", "Dept_id", "Hrs", "MinDegree", "Name" },
                values: new object[,]
                {
                    { 1, 100, 1, 60, 50, "C# Programming" },
                    { 2, 100, 2, 40, 50, "Database Design" }
                });

            migrationBuilder.InsertData(
                table: "Trainees",
                columns: new[] { "Id", "Address", "Dept_id", "Grade", "Image", "Name" },
                values: new object[,]
                {
                    { 1, "Giza", 1, 85m, "ali.jpg", "Ali Hassan" },
                    { 2, "Cairo", 2, 92m, "noha.jpg", "Noha Fathy" }
                });

            migrationBuilder.InsertData(
                table: "CrsResults",
                columns: new[] { "Id", "Crs_id", "Degree", "Trainee_id" },
                values: new object[,]
                {
                    { 1, 1, 85, 1 },
                    { 2, 2, 92, 2 }
                });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Address", "Crs_id", "Dept_id", "Image", "Name", "Salary" },
                values: new object[,]
                {
                    { 1, "Cairo", 1, 1, "mohamed.jpg", "Mohamed Ali", 5000m },
                    { 2, "Alexandria", 2, 2, "mona.jpg", "Mona Zaki", 6000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Dept_id",
                table: "Courses",
                column: "Dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_CrsResults_Crs_id",
                table: "CrsResults",
                column: "Crs_id");

            migrationBuilder.CreateIndex(
                name: "IX_CrsResults_Trainee_id",
                table: "CrsResults",
                column: "Trainee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_Crs_id",
                table: "Instructors",
                column: "Crs_id");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_Dept_id",
                table: "Instructors",
                column: "Dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_Dept_id",
                table: "Trainees",
                column: "Dept_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrsResults");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Trainees");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
